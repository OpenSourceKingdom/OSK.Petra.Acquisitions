using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the state of a hold
/// </summary>
public class AcquisitionHoldState
{
    #region Variables

    /// <summary>
    /// The acquirer to which the hold belongs
    /// </summary>
    public required IAcquirer Acquirer { get; init; }

    /// <summary>
    /// The provisions reserved for the acquireable that the hold belongs
    /// </summary>
    public IEnumerable<Provision> ReservedProvisions { get; init; } = [];

    /// <summary>
    /// The hold entity for the specific hold
    /// </summary>
    public required IAcquisitionHold Hold { get; init; }

    #endregion
}
