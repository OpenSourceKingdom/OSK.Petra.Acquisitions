using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Provides insights into the current state of acquisition when starting an acquisition process
/// </summary>
/// <param name="acquirer">The acquirer for the acquisition</param>
/// <param name="acquireable">The acquireable being acquired</param>
/// <param name="acquiredAmount">The amount of the acquireable currently acquired</param>
/// <param name="acquisitionsInProgress">The amount of acquireables currently in the queue in some form</param>
/// <param name="groupSize">The total hubs that are in the same group acquiring the acquireable</param>
/// <param name="services">The services for the acquisition process</param>
public class AcquisitionProcessContext(IAcquirer acquirer, IAcquireable acquireable, long acquiredAmount, int acquisitionsInProgress, int groupSize, IServiceProvider services)
{
    public IServiceProvider Services { get; } = services; 

    public IAcquirer Acquirer { get; } = acquirer;

    public IAcquireable Acquireable { get; } = acquireable;

    public int TotalAcquisitionsInProgress { get; } = acquisitionsInProgress;

    public int GroupSize { get; } = groupSize;

    public long AcquiredAmount { get; } = acquiredAmount;
}
