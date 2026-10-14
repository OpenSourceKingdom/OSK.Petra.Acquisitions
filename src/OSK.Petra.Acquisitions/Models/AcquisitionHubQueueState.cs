using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the queue state for a given hub
/// </summary>
public class AcquisitionHubQueueState
{
    #region Variables
    
    /// <summary>
    /// Represents the total number of acquisitions of the related acquireable this hub has generated
    /// </summary>
    public int TotalQueuedAcquisitions => PendingAcquisitions.Count + AcquisitionsInProgress.Count;

    /// <summary>
    /// THe states of the pending acquisitions
    /// </summary>
    public LinkedList<AcquisitionHoldState> PendingAcquisitions { get; init; } = [];

    /// <summary>
    /// The states of the acquisitions in progress
    /// </summary>
    public List<AcquisitionHoldState> AcquisitionsInProgress { get; init; } = [];

    #endregion

    #region Api

    /// <summary>
    /// Triggers the queue state to transition a pending acquisition to a start state 
    /// </summary>
    public void StartNextAcquisition()
    {
        if (PendingAcquisitions.Count is 0)
        {
            return;
        }

        AcquisitionsInProgress.Add(PendingAcquisitions.First.Value);
        PendingAcquisitions.RemoveFirst();
    }

    /// <summary>
    /// Adds the given hold state to the pending queue
    /// </summary>
    /// <param name="acquisitionHoldState">The state that is pending</param>
    /// <param name="isQueued">Is the hold state for an acquisition currently in progress or queued to occur later</param>
    public void AddPendingAcquisition(AcquisitionHoldState acquisitionHoldState, bool isQueued)
    {
        ICollection<AcquisitionHoldState> holdTargetCollection = isQueued
            ? PendingAcquisitions
            : AcquisitionsInProgress;

        holdTargetCollection.Add(acquisitionHoldState);
    }

    /// <summary>
    /// Removes the queue information for a given acquirer for the acquisition hub
    /// </summary>
    /// <param name="acquirer">Thq acquireer that is trying to remove the acquisition</param>
    /// <returns>The hold state for the acquisition that was removed</returns>
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
