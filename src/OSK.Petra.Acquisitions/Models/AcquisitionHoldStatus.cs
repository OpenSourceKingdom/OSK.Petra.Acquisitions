namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the various statuses a hold can be in
/// </summary>
public enum AcquisitionHoldStatus
{
    /// <summary>
    /// The hold is currently active and being processed
    /// </summary>
    Active,

    /// <summary>
    /// The hold has completed and is no longer preventing an acquisition
    /// </summary>
    Released,

    /// <summary>
    /// The hold has been blocked and the current acquisition will be terminated
    /// </summary>
    Blocked
}
