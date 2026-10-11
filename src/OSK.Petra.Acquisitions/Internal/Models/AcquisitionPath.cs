using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Options;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionPath(AcquisitionHubGroup hubGroup, IAcquisitionHub hub, IAcquirer acquirer, AcquisitionInformation information, AcquisitionPathState state) : Acquisition, IAcquisitionPath
{
    #region IAcquisitionPath

    public AcquisitionPathType PathType { get; } = information?.PathType ?? throw new ArgumentNullException(nameof(information.PathType));

    public IAcquireable RootAcquireable => information.RootDefinition.Acquireable;

    public int TotalTiers => information.Choices.Count;

    public int TotalChoices => information.MaxTier;

    public IEnumerable<IAcquisitionChoice> GetChoices(AcquisitionPathSearchOptions? searchOptions = null)
    {
        var searchableChoices = searchOptions is { TierFilter.Length: >= 1 }
            ? information.Choices.Values.Where(v => searchOptions.Value.TierFilter.Contains(v.Tier)).Select(v => v.Definition)
            : information.Choices.Values.Select(v => v.Definition);

        var acquisitionChoices = searchableChoices.Select(choice => SharedAcquisitionPool.GetAcquisition(hubGroup, hub, acquirer, information, choice, state)).Cast<IAcquisitionChoice>();
        return searchOptions is { PredicateFilters.Length: >= 1 }
            ? acquisitionChoices.Where(choice => searchOptions.Value.PredicateFilters.All(predicate => predicate(choice)))
            : acquisitionChoices;
    }

    #endregion

    #region Acquisition Overrides

    public override void Update(IEnumerable<Provision> provisions)
    {
        foreach (var choice in GetChoices().OfType<AcquisitionChoice>())
        {
            choice.Update(provisions);
        }
    }

    #endregion
}
