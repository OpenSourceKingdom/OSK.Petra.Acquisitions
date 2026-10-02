using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Acquisitions.Requirements.Dependency;

public readonly struct AcquisitionDependency(Guid acquireableId, long requiredAcquisitionCount)
{
    public readonly Guid AcquireableId { get; } = acquireableId;

    public readonly long RequiredAcquisitionCount { get; } = requiredAcquisitionCount;
}
