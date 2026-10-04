using System;

namespace OSK.Petra.Acquisitions.Requirements.Dependency;

public readonly struct AcquisitionDependency(Guid acquisitionPathId, Guid acquireableId, long requiredAcquisitionCount)
{
    #region Variables

    public readonly Guid AcquisitionPathId { get; } = acquisitionPathId;

    public readonly Guid AcquireableId { get; } = acquireableId;

    public readonly long RequiredAcquisitionCount { get; } = requiredAcquisitionCount;

    #endregion

    #region Constructors

    public AcquisitionDependency(Guid acquireableId, long requiredAcquisitionCount)
        : this(acquireableId, acquireableId, requiredAcquisitionCount)
    {
    }

    #endregion
}
