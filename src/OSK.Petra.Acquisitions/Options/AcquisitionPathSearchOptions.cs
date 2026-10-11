using OSK.Petra.Acquisitions.Models;
using System;
using System.Linq;

namespace OSK.Petra.Acquisitions.Options;

public readonly struct AcquisitionPathSearchOptions
{
    #region Variables

    public int[]? TierFilter { get; init; }

    public Func<IAcquisitionChoice, bool>[]? PredicateFilters { get; init; }

    #endregion

    #region Api

    public AcquisitionPathSearchOptions AvailableEdges()
        => Where(choice => choice.NextChoices is null || choice.NextChoices.Count is 0 || choice.NextChoices.All(next => !next.IsAvailable));

    public AcquisitionPathSearchOptions IncludeComplete()
        => Where(choice => choice.IsComplete);

    public AcquisitionPathSearchOptions IncludeAvailable()
        => Where(choice => choice.IsAvailable);

    public AcquisitionPathSearchOptions IncludeUnavailable()
        => Where(choice => choice.IsAvailable);

    public AcquisitionPathSearchOptions IncludeIncomplete()
        => Where(choice => choice.IsAvailable);

    public AcquisitionPathSearchOptions ExcludeComplete()
        => Where(choice => !choice.IsComplete);

    public AcquisitionPathSearchOptions ExcludeIncomplete()
        => Where(choice => !choice.IsComplete);

    public AcquisitionPathSearchOptions ExcludeAvailable()
        => Where(choice => !choice.IsAvailable);

    public AcquisitionPathSearchOptions ExcludeUnavailable()
        => Where(choice => !choice.IsAvailable);

    public AcquisitionPathSearchOptions WithTiers(params int[] tiers)
        => new()
        {
            TierFilter = [..TierFilter is null
                ? tiers
                : TierFilter.Concat(tiers).Distinct()],
            PredicateFilters = PredicateFilters
        };

    public AcquisitionPathSearchOptions Where(Func<IAcquisitionChoice, bool> predicate)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return new AcquisitionPathSearchOptions()
        {
            PredicateFilters = PredicateFilters is null
                ? [predicate]
                : [.. PredicateFilters.Append(predicate)]
        };
    }

    #endregion
}
