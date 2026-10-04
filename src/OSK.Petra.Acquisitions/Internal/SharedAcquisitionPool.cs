using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal;

internal static class SharedAcquisitionPool
{
    #region Variables

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
            return new(definition);
        }

        // If a single path, each choice can only have a single next choice with the final choice having nothing. So, if any have more than one next choice, then it is a multi-path acquisition.
        var pathType = choiceTiers.Any(tier => tier.Value.Any(choice => choice.NextChoices.Count > 1))
            ? AcquisitionPathType.Multi
            : AcquisitionPathType.Single;

        information = new AcquisitionInformation(definition, pathType, choiceTiers);
        _choiceInformationLookup[definition.Acquireable.Id] = information;

        return information;
    }

    #endregion

    #region Helpers

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

        foreach (var child in acquisition.NextChoices)
        {
            DiscoverChoiceTier(child, tier + 1, choiceTierLookup);
        }
    }

    #endregion
}
