using System;

namespace OSK.Petra.Acquisitions.Requirements.Dependency;

/// <summary>
/// Defines a dependency that must be validated for an <see cref="AcquisitionDependencyRequirement"/>
/// </summary>
/// <param name="acquisitionPathId">The path id for the dependency</param>
/// <param name="acquireableId">The acquireable that is depended upon</param>
/// <param name="requiredAcquisitionCount">The amount of acquisitions required before the dependency is satisfied</param>
public readonly struct AcquisitionDependency(Guid acquisitionPathId, Guid acquireableId, long requiredAcquisitionCount)
{
    #region Variables

    /// <summary>
    /// The path id for the dependency
    /// </summary>
    public readonly Guid AcquisitionPathId { get; } = acquisitionPathId;

    /// <summary>
    /// The acquireable that is depended upon
    /// </summary>
    public readonly Guid AcquireableId { get; } = acquireableId;

    /// <summary>
    /// The amount of acquisitions required before the dependency is satisfied
    /// </summary>
    public readonly long RequiredAcquisitionCount { get; } = requiredAcquisitionCount;

    #endregion

    #region Constructors

    public AcquisitionDependency(Guid acquireableId, long requiredAcquisitionCount)
        : this(acquireableId, acquireableId, requiredAcquisitionCount)
    {
    }

    #endregion
}
