using OSK.Petra.Acquisitions.Ports;
using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Provides insights into the current state of acquisition when starting an acquisition process
/// </summary>
/// <param name="hub">The hub where the acquisition initiation occurred</param>
/// <param name="acquirer">The acquirer for the acquisition</param>
/// <param name="acquireable">The acquireable being acquired</param>
/// <param name="acquiredAmount">The amount of the acquireable currently acquired</param>
/// <param name="acquisitionsInProgress">The amount of acquireables currently in the queue in some form</param>
/// <param name="groupSize">The total hubs that are in the same group acquiring the acquireable</param>
/// <param name="services">The services for the acquisition process</param>
public class HubAcquisitionContext(IAcquisitionHub hub, IAcquirer acquirer, IAcquireable acquireable, long acquiredAmount, int acquisitionsInProgress, int groupSize, IServiceProvider services)
{
    /// <summary>
    /// The services for the acquisition process
    /// </summary>
    public IServiceProvider Services { get; } = services;

    /// <summary>
    /// The hub where the acquisition initiation occurred
    /// </summary>
    public IAcquisitionHub Hub { get; } = hub;

    /// <summary>
    /// The acquirer for the acquisition
    /// </summary>
    public IAcquirer Acquirer { get; } = acquirer;

    /// <summary>
    /// The acquireable being acquired
    /// </summary>
    public IAcquireable Acquireable { get; } = acquireable;

    /// <summary>
    /// The amount of acquireables currently in the queue in some form
    /// </summary>
    public int TotalAcquisitionsInProgress { get; } = acquisitionsInProgress;

    /// <summary>
    /// The total hubs that are in the same group acquiring the acquireable
    /// </summary>
    public int GroupSize { get; } = groupSize;

    /// <summary>
    /// The amount of the acquireable currently acquired
    /// </summary>
    public long AcquiredAmount { get; } = acquiredAmount;
}
