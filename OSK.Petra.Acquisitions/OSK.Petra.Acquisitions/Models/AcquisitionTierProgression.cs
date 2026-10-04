using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Acquisitions.Models;

public readonly struct AcquisitionTierProgression(Guid acquireableId, long totalAcquisitions)
{
    public Guid AcquireableId { get; } = acquireableId;

    public long TotalAcquisitions { get; } = totalAcquisitions;
}
