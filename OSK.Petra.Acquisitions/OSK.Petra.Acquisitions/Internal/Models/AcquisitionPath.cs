using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Options;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionPath(AcquisitionHubGroup hubGroup, IAcquisitionHub hub, IAcquirer acquirer, AcquisitionInformation information, AcquisitionPathState state) : Acquisition, IAcquisitionPath
{
    #region IAcquisitionPath

    public AcquisitionPathType PathType { get; } = information?.PathType ?? throw new ArgumentNullException(nameof(information.PathType));

    public IAcquireable RootAcquireable => information.RootDefinition.Acquireable;

    public int TotalTiers => information.ChoiceTiers.Count;

    public int TotalChoices { get; } = information.ChoiceTiers.Sum(tier => tier.Value.Count);

    public IEnumerable<IAcquisitionChoice> GetChoices(AcquisitionPathSearchOptions? searchOptions = null)
    {
        var searchableChoiceTiers = searchOptions is { TierFilter.Length :>= 1 }
            ? searchOptions.Value.TierFilter.Where(tier => information.ChoiceTiers.TryGetValue(tier, out _)).SelectMany(tier => information.ChoiceTiers[tier])
            : information.ChoiceTiers.SelectMany(kvp => kvp.Value);

        var acquisitionChoices = searchableChoiceTiers.Select(choice => information.ToAcquisition(hubGroup, hub, acquirer, state)).Cast<IAcquisitionChoice>();
        return searchOptions is { PredicateFilters.Length: >= 1 }
            ? acquisitionChoices.Where(choice => searchOptions.Value.PredicateFilters.All(predicate => predicate(choice)))
            : acquisitionChoices;
    }

    #endregion
}
