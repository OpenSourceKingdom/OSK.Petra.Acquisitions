using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the result of a cancellation of an acquisition
/// </summary>
public class AcquisitionCancellationResult
{
    #region Variables

    /// <summary>
    /// Describes if cancellation succeeded. This may fail if no acquisitions are queued, etc.
    /// </summary>
    public bool CancellationSuccessful { get; }

    /// <summary>
    /// The collection of provisions that are freed by the cancellation of the acquisition.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The freed provisions are sent to the hub that was processing the acquisition to handle potential refunds, so be sure hub logic does not handle refunds or adding these back to a user 
    /// may result in double refunding.</item>
    /// </list>
    /// </remarks>
    public IReadOnlyCollection<Provision> FreedProvisions { get; }

    #endregion

    #region Constructors

    internal AcquisitionCancellationResult(bool cancellationSuccessful, IEnumerable<Provision> freedProvisions)
    {
        CancellationSuccessful = cancellationSuccessful;
        FreedProvisions = freedProvisions?.ToArray() ?? throw new ArgumentNullException(nameof(freedProvisions));
    }

    #endregion
}
