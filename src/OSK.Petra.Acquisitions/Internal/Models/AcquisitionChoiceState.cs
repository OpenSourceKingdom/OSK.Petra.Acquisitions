using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionChoiceState
{
    #region Variables

    public required Guid AcquireableId { get; init; }

    public long TotalAcquisitions { get; set; }

    public Dictionary<Guid, AcquisitionHubQueueState> HubQueues { get; init; } = [];

    #endregion

    #region Api

    public bool TryGetHubQueue(Guid hubId, [NotNullWhen(true)] out AcquisitionHubQueueState? hubQueue)
        => HubQueues.TryGetValue(hubId, out hubQueue);

    public AcquisitionHubQueueState GetOrAddHubQueue(Guid hubId)
    {
        if (!HubQueues.TryGetValue(hubId, out var hubQueue))
        {
            hubQueue = new();
            HubQueues[hubId] = hubQueue;
        }

        return hubQueue;
    }

    public int GetTotalAcquisitionsInProgress(Guid hubId)
        => HubQueues.TryGetValue(hubId, out var hubQueue)
            ? hubQueue.AcquisitionsInProgress.Count
            : 0;

    public int GetTotalPendingAcquisitions(Guid hubId)
        => HubQueues.TryGetValue(hubId, out var hubQueue)
            ? hubQueue.PendingAcquisitions.Count
            : 0;

    public bool RemoveHubQueue(Guid hubId)
        => HubQueues.Remove(hubId);

    #endregion
}