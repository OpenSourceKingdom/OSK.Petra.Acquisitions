using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionHubQueueState
{
    #region Variables

    public int TotalQueuedAcquisitions => PendingAcquisitions.Count + AcquisitionsInProgress.Count;

    public LinkedList<AcquisitionHoldState> PendingAcquisitions { get; init; } = [];

    public List<AcquisitionHoldState> AcquisitionsInProgress { get; init; } = [];

    #endregion

    #region Api

    public void StartNextAcquisition()
    {
        AcquisitionsInProgress.Add(PendingAcquisitions.First.Value);
        PendingAcquisitions.RemoveFirst();
    }

    public void AddPendingAcquisition(AcquisitionHoldState acquisitionHoldState, bool isQueued)
    {
        ICollection<AcquisitionHoldState> holdTargetCollection = isQueued
            ? PendingAcquisitions
            : AcquisitionsInProgress;

        holdTargetCollection.Add(acquisitionHoldState);
    }

    public AcquisitionHoldState? RemoveAcquisition(IAcquirer acquirer)
    {
        var pendingMatch = PendingAcquisitions.LastOrDefault(state => state.Acquirer == acquirer);
        if (pendingMatch is not null)
        {
            PendingAcquisitions.Remove(pendingMatch);
            return pendingMatch;
        }

        var inProgressMatch = AcquisitionsInProgress.LastOrDefault(state => state.Acquirer == acquirer);
        if (inProgressMatch != null)
        {
            AcquisitionsInProgress.Remove(inProgressMatch);
            return inProgressMatch;
        }

        return null;
    }

    #endregion
}
