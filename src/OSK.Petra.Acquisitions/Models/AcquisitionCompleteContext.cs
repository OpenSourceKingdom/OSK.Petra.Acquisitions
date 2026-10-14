using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// A context providing insighting into a completed acquisition process
/// </summary>
/// <param name="acquirer">The acquirer who initiated the acquisition</param>
/// <param name="pathType">The type of path the acquireable belonged to, if any</param>
/// <param name="pathAcquisition">The path acquisition root to this acquireable</param>
/// <param name="acquireable">The acquired thing</param>
/// <param name="totalAcquisitions">The total amount acquired</param>
/// <param name="success">Whether this completion context was a successful acquisition</param>
/// <param name="services">The services assocaited with the acquisition process</param>
public class AcquisitionCompleteContext(IAcquirer acquirer, AcquisitionPathType? pathType, IAcquireable pathAcquisition, IAcquireable acquireable, long totalAcquisitions, bool success, IServiceProvider services)
{
    /// <summary>
    /// Describes if the acquireable belongs to a path or a single choice
    /// </summary>
    public bool IsPath => pathType is not null;

    /// <summary>
    /// The services assocaited with the acquisition process
    /// </summary>
    public IServiceProvider Services { get; } = services;

    /// <summary>
    /// Whether this completion context was a successful acquisition
    /// </summary>
    public bool Success { get; } = success;

    /// <summary>
    /// The acquirer who initiated the acquisition
    /// </summary>
    public IAcquirer Acquirer { get; } = acquirer;

    /// <summary>
    /// The acquireable id that is the path root
    /// </summary>
    public Guid PathId { get; } = pathAcquisition.Id;

    /// <summary>
    /// The acquired thing
    /// </summary>
    public IAcquireable Acquireable { get; } = acquireable;

    /// <summary>
    /// The services assocaited with the acquisition process
    /// </summary>
    public long TotalAcquisitions { get; } = totalAcquisitions;
}
