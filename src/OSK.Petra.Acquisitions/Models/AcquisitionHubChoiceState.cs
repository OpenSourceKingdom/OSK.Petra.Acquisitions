using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the state of a particular choice for a hub
/// </summary>
public class AcquisitionHubChoiceState
{
    /// <summary>
    /// The id of the acquisition path
    /// </summary>
    public required Guid PathId { get; init; }

    /// <summary>
    /// The id of the acquireable being acquired
    /// </summary>
    public required Guid AcquireableId { get; init; }

    /// <summary>
    /// The total number of acquisitions that have been performed
    /// </summary>
    public long TotalAcquisitions { get; init; }

    /// <summary>
    /// The state of the queue for this hub
    /// </summary>
    public required AcquisitionHubQueueState QueueState { get; init; }
}
