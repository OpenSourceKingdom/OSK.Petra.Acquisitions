using OSK.Petra.Acquisitions.Options;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// An acquisition choice that can be acquired within a <see cref="IAcquisitionView"/>
/// </summary>
public interface IAcquisitionChoice: IAcquisition
{
    /// <summary>
    /// The acquireable this choice is for
    /// </summary>
    IAcquireable Acquireable { get; }

    /// <summary>
    /// The acquisition tier in a path
    /// </summary>
    int Tier { get; }

    /// <summary>
    /// The total amount of acquisitions for this acquireable that are currently queued
    /// </summary>
    int TotalQueued { get; }

    /// <summary>
    /// The total amount of acquisitions for this acquireable that are currently processing
    /// </summary>
    int TotalInProgress { get; }

    /// <summary>
    /// Whether this acquisition choice is available for acquisition
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Whether this acquisition choice has reached the maximum acquisitions allowed
    /// </summary>
    bool IsComplete { get; }

    /// <summary>
    /// Gets the details for the current queue of acquisitions being processed
    /// </summary>
    /// <returns></returns>
    IEnumerable<AcquisitionProgress> GetProgresses();

    /// <summary>
    /// The provision expendeiture cost summary to start the next acquisition
    /// </summary>
    ProvisionExpenditureSummary NextExpenditure { get; }

    /// <summary>
    /// Initiates the acquisition process using the provided provisions
    /// </summary>
    /// <param name="provisions">The provisions available for the acquisition process to use</param>
    /// <param name="options">Extra options related to the acquisition</param>
    /// <returns>The result of the acquisition process</returns>
    AcquisitionResult InitiateAcquisition(IEnumerable<Provision> provisions, AcquisitionOptions? options = null);

    /// <summary>
    /// Cancels an acquisition that is either queued or in progress
    /// </summary>
    /// <returns>The result of the acuqisition cancellation</returns>
    AcquisitionCancellationResult CancelAcquisition();

    /// <summary>
    /// The collection of acquisition choices that follow this one
    /// </summary>
    IReadOnlyCollection<IAcquisitionChoice> PreviousChoices { get; }

    /// <summary>
    /// The collection of acquisition choices that precede this one
    /// </summary>
    IReadOnlyCollection<IAcquisitionChoice> NextChoices { get; }
}
