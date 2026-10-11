using System;

namespace OSK.Petra.Acquisitions.Models;

public readonly struct AcquisitionTierProgression(Guid acquireableId, long totalAcquisitions)
{
    public Guid AcquireableId { get; } = acquireableId;

    public long TotalAcquisitions { get; } = totalAcquisitions;
}
