using OSK.Petra.Acquisitions.Models;
using System;
using System.Linq;

namespace OSK.Petra.Acquisitions.Options;

/// <summary>
/// A search option filter that is used to get a list of acquisitions from an acquisition path
/// </summary>
public readonly struct AcquisitionPathSearchOptions
{
    #region Variables

    /// <summary>
    /// If set, filters the returned choices based on tier level.
    /// </summary>
    public int[]? TierFilter { get; init; }

    /// <summary>
    /// A collection of predicates to utilize when searching a given acquisition path to filter out acquisition choices
    /// </summary>
    public Func<IAcquisitionChoice, bool>[]? PredicateFilters { get; init; }

    #endregion

    #region Api

    /// <summary>
    /// Search for the available edges of an acquisition path. i.e. this is the choices that are at the highest tier achieved by a player that are available
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions AvailableEdges()
        => Where(choice => choice.IsAvailable && (choice.NextChoices is null || choice.NextChoices.Count is 0 || choice.NextChoices.All(next => !next.IsAvailable)));

    /// <summary>
    /// Includes the completed acquisition choices in the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions IncludeComplete()
        => Where(choice => choice.IsComplete);

    /// <summary>
    /// Includes available acquisition choices in the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions IncludeAvailable()
        => Where(choice => choice.IsAvailable);

    /// <summary>
    /// Includes unavailable acquisition choices in the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions IncludeUnavailable()
        => Where(choice => choice.IsAvailable);

    /// <summary>
    /// Includes incomplete acquisition choices in the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions IncludeIncomplete()
        => Where(choice => choice.IsAvailable);

    /// <summary>
    /// Excludes complete acquisition choices from the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions ExcludeComplete()
        => Where(choice => !choice.IsComplete);

    /// <summary>
    /// Excludes incomplete acquisition choices from the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions ExcludeIncomplete()
        => Where(choice => !choice.IsComplete);

    /// <summary>
    /// Excludes availablie acquisition choices from the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions ExcludeAvailable()
        => Where(choice => !choice.IsAvailable);

    /// <summary>
    /// Excludes unavailable acquisition choices from the search
    /// </summary>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions ExcludeUnavailable()
        => Where(choice => !choice.IsAvailable);

    /// <summary>
    /// Filters acquisition choices with the provided tiers
    /// </summary>
    /// <param name="tiers">The acquisition tiers to filter</param>
    /// <returns>The options for chaining</returns>
    public AcquisitionPathSearchOptions WithTiers(params int[] tiers)
        => new()
        {
            TierFilter = [..TierFilter is null
                ? tiers
                : TierFilter.Concat(tiers).Distinct()],
            PredicateFilters = PredicateFilters
        };

    /// <summary>
    /// Adds a custom search predicate filter to the search
    /// </summary>
    /// <param name="predicate">The predicate that will be used to filter choices with a given search</param>
    /// <returns>The options for chaining</returns>
    /// <exception cref="ArgumentNullException">If the predicate is null</exception>
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
