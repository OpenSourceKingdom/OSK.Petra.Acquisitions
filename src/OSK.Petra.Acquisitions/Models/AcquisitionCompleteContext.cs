using System;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionCompleteContext(IAcquirer acquirer, AcquisitionPathType? pathType, IAcquireable parentAcquireable, IAcquireable acquireable, long totalAcquired, bool success, IServiceProvider services)
{
    public bool IsPath => pathType is not null;

    public IServiceProvider Services { get; } = services;

    public bool Success { get; } = success;

    public IAcquirer Acquirer { get; } = acquirer;

    public Guid PathId { get; } = parentAcquireable.Id;

    public IAcquireable Acquireable { get; } = acquireable;

    public long TotalAcquired { get; } = totalAcquired;
}
