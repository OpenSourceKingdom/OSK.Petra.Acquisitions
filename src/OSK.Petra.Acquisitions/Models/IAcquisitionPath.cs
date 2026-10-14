using OSK.Petra.Acquisitions.Options;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// An acquisition that has more than a single choice within it
/// </summary>
public interface IAcquisitionPath: IAcquisition
{
    /// <summary>
    /// The path type this acquisition possesses
    /// </summary>
    AcquisitionPathType PathType { get; }

    /// <summary>
    /// The paths starting acquireable
    /// </summary>
    IAcquireable RootAcquireable { get; }

    /// <summary>
    /// The total number of tiers in the path
    /// </summary>
    int TotalTiers { get; }

    /// <summary>
    /// The total number of choices in the path
    /// </summary>
    int TotalChoices { get; }

    /// <summary>
    /// Searches the path for a collection of choices, given search options
    /// </summary>
    /// <param name="searchOptions">The search options to filter the path choices</param>
    /// <returns>The choices in the path that match the search options</returns>
    IEnumerable<IAcquisitionChoice> GetChoices(AcquisitionPathSearchOptions? searchOptions = null);
}
