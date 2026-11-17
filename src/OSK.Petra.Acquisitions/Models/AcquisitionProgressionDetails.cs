using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Details the progression of an acquisition
/// </summary>
/// <param name="acquireableId">The id of the acquireable this progression is associated with</param>
/// <param name="totalAcquisitions">The total acquisitions of the acquireable</param>
public readonly struct AcquisitionProgressionDetails(Guid acquireableId, long totalAcquisitions)
{
    /// <summary>
    /// The id of the acquireable this progression is associated with
    /// </summary>
    public Guid AcquireableId { get; } = acquireableId;

    /// <summary>
    /// The total acquisitions of the acquireable
    /// </summary>
    public long TotalAcquisitions { get; } = totalAcquisitions;
}
