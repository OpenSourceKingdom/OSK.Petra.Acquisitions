namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes various outcomes for an acquisition process
/// </summary>
public enum AcquisitionOutcome
{
    /// <summary>
    /// The acquisition fully completed successfully
    /// </summary>
    Success,
    /// <summary>
    /// The acquisition process requires a delay before completing successfully
    /// </summary>
    Delayed,

    /// <summary>
    /// There is not enough provisions to complete the acquisition
    /// </summary>
    InsufficientProvisions,

    /// <summary>
    /// The maximum number of acquisitions that can be queued has been reached
    /// </summary>
    MaxQueueSizeReached,
    /// <summary>
    /// The maximum number of acuisitions for the acquireable has been reached
    /// </summary>
    MaxAcquisitionsReached
}
