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
    /// The collection of provisions that are freed by the cancellation of the acquisition
    /// </summary>
    public IReadOnlyCollection<Provision> FreedProvisions { get; }

    #endregion

    #region Constructors

    internal AcquisitionCancellationResult(IEnumerable<Provision> freedProvisions)
    {
        FreedProvisions = freedProvisions?.ToArray() ?? throw new ArgumentNullException(nameof(freedProvisions));
    }

    #endregion
}
