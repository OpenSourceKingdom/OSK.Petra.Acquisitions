using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OSK.Petra.Acquisitions.Internal;

internal static class SharedAcquisitionPool
{
    #region Variables

    private readonly record struct ChoiceSetKey(Guid HubId, IAcquirer Acquirer, Guid RootId);
    private static readonly ConditionalWeakTable<AcquisitionHubGroup, Dictionary<ChoiceSetKey, Dictionary<Guid, AcquisitionChoice>>> _choiceCache = new();

    private static readonly Dictionary<Guid, AcquisitionInformation> _choiceInformationLookup = [];

    #endregion

    #region Api

    public static AcquisitionInformation GetInformation(IAcquisitionDefinition definition)
    {
        if (_choiceInformationLookup.TryGetValue(definition.Acquireable.Id, out var information))
        {
            return information;
        }

        var choiceTiers = BuildChoiceTiers(definition);
        if (choiceTiers.Count is 1)
        {
            _choiceInformationLookup[definition.Acquireable.Id] = new(definition);
            return _choiceInformationLookup[definition.Acquireable.Id];
        }

        // If a single path, each choice can only have a single next choice with the final choice having nothing. So, if any have more than one next choice, then it is a multi-path acquisition.
        var pathType = choiceTiers.Any(tier => tier.Value.Any(choice => choice.NextChoices.Count > 1))
            ? AcquisitionPathType.Multi
            : AcquisitionPathType.Single;

        information = new AcquisitionInformation(definition, pathType, choiceTiers.SelectMany(kvp => kvp.Value.Select(definition => new ValueTuple<int, IAcquisitionDefinition>(kvp.Key, definition))));
        _choiceInformationLookup[definition.Acquireable.Id] = information;

        return information;
    }

    public static Acquisition GetAcquisition(AcquisitionHubGroup group, IAcquisitionHub hub, IAcquirer acquirer, AcquisitionInformation acquisitionInformation, IAcquisitionDefinition choiceDefinition, AcquisitionPathState pathState, bool isPathView)
    {
        var id = choiceDefinition.Acquireable.Id;
        var isRoot = acquisitionInformation.RootDefinition.Acquireable.Id == id;

        if (isRoot && !isPathView && choiceDefinition.NextChoices is { Count: > 0 })
        {
            return new AcquisitionPath(group, hub, acquirer, acquisitionInformation, pathState);
        }

        return GetChoiceSet(group, hub, acquirer, acquisitionInformation, pathState)[id];
    }

    public static void Evict(AcquisitionHubGroup group, Guid hubId)
    {
        if (!_choiceCache.TryGetValue(group, out var perGroup))
        {
            return;
        }

        foreach (var key in perGroup.Keys.Where(k => k.HubId == hubId).ToList())
        {
            perGroup.Remove(key);
        }
    }

    #endregion

    #region Helpers

    private static Dictionary<Guid, AcquisitionChoice> GetChoiceSet(AcquisitionHubGroup group, IAcquisitionHub hub, IAcquirer acquirer, AcquisitionInformation information, AcquisitionPathState pathState)
    {
        var perGroup = _choiceCache.GetOrCreateValue(group);
        var key = new ChoiceSetKey(hub.Id, acquirer, information.RootDefinition.Acquireable.Id);

        if (perGroup.TryGetValue(key, out var cached))
        {
            return cached;
        }

        // 1. One instance per choice
        var choices = new Dictionary<Guid, AcquisitionChoice>(information.Choices.Count);
        foreach (var (id, (tier, definition)) in information.Choices)
        {
            choices[id] = new AcquisitionChoice(group, hub, acquirer, information.RootDefinition, definition, tier, pathState.GetOrCreateState(id));
        }

        // 2. Wire links between those same instances
        foreach (var (id, choice) in choices)
        {
            var definition = information.Choices[id].Definition;

            choice.NextChoices = [.. (definition.NextChoices ?? []).Select(next => Resolve(choices, next, id))];
            choice.PreviousChoices = [.. (definition.PreviousChoices ?? []).Select(previous => Resolve(choices, previous, id))];
        }

        perGroup[key] = choices;
        return choices;
    }

    private static AcquisitionChoice Resolve(Dictionary<Guid, AcquisitionChoice> choices, IAcquisitionDefinition linked, Guid ownerId)
        => choices.TryGetValue(linked.Acquireable.Id, out var choice)
            ? choice
            : throw new InvalidOperationException($"Acquisition '{ownerId}' links to '{linked.Acquireable.Id}', which is not part of its path.");

    public static Dictionary<int, IReadOnlyCollection<IAcquisitionDefinition>> BuildChoiceTiers(IAcquisitionDefinition root)
    {
        var choiceTierLookup = new Dictionary<IAcquisitionDefinition, int>();
        DiscoverChoiceTier(root, 0, choiceTierLookup);

        var choiceTiers = new Dictionary<int, IReadOnlyCollection<IAcquisitionDefinition>>();
        foreach (var choiceGroup in choiceTierLookup.GroupBy(kvp => kvp.Value))
        {
            choiceTiers[choiceGroup.Key] = [ ..choiceGroup.Select(kvp => kvp.Key) ];
        }

        return choiceTiers;
    }

    private static void DiscoverChoiceTier(IAcquisitionDefinition acquisition, int tier, Dictionary<IAcquisitionDefinition, int> choiceTierLookup)
    {
        // Only update if we've found a deeper path to this acquisition.
        if (choiceTierLookup.TryGetValue(acquisition, out var existingTier) && existingTier >= tier)
        {
            return;
        }

        choiceTierLookup[acquisition] = tier;

        if (acquisition.NextChoices is null)
        {
            return;
        }

        foreach (var child in acquisition.NextChoices)
        {
            DiscoverChoiceTier(child, tier + 1, choiceTierLookup);
        }
    }

    #endregion
}
