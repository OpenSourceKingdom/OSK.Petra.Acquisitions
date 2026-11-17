using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// A context providing insight into a completed acquisition process. This does not indicate the acquisition completed successfully. Please use <see cref="Success"/> to determine if the acquisition 
/// finished successfully.
/// </summary>
/// <param name="acquirer">The acquirer who initiated the acquisition</param>
/// <param name="pathType">The type of path the acquireable belonged to, if any</param>
/// <param name="pathAcquisition">The path acquisition root to this acquireable</param>
/// <param name="acquireable">The acquired thing</param>
/// <param name="totalAcquisitions">The total amount acquired</param>
/// <param name="provisions">The provisions used to acquire the choice</param>
/// <param name="success">Whether this completion context was a successful acquisition</param>
/// <param name="services">The services associated with the acquisition process</param>
public class AcquisitionCompleteContext(IAcquirer acquirer, AcquisitionPathType? pathType, IAcquireable pathAcquisition, IAcquireable acquireable, long totalAcquisitions, IEnumerable<Provision> provisions, bool success, IServiceProvider services)
{
    /// <summary>
    /// Describes if the acquireable belongs to a path or a single choice
    /// </summary>
    public bool IsPath => pathType is not null;

    /// <summary>
    /// The services associated with the acquisition process
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
    /// The total acquisitions for the choice
    /// </summary>
    public long TotalAcquisitions { get; } = totalAcquisitions;

    /// <summary>
    /// The total provisions used to acquire the choice. In the event an acquisition failed, these should be considered for refunding
    /// </summary>
    public IEnumerable<Provision> UsedProvisions { get; } = provisions;
}
