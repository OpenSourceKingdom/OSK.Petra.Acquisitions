using OSK.Petra.Provisions.Models;
using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the outcome of an acquisition process
/// </summary>
public class AcquisitionResult
{
    #region Static

    /// <summary>
    /// Creates an acquisition result for a successfully acquired thing
    /// </summary>
    /// <param name="acquireable">The thing that was acquired</param>
    /// <param name="expenditureSummary">The provision expenditure for the acquireable</param>
    /// <returns>The result</returns>
    public static AcquisitionResult Acquired(IAcquireable acquireable, ProvisionExpenditureSummary? expenditureSummary = null) => new(AcquisitionOutcome.Success, acquireable, expenditureSummary);

    /// <summary>
    /// Creates an acquisition result for an acquisition that will be delayed
    /// </summary>
    /// <param name="expenditureSummary">The provision expenditure for the acquireable</param>
    /// <returns>The result</returns>
    public static AcquisitionResult Delayed(ProvisionExpenditureSummary? expenditureSummary = null) => new(AcquisitionOutcome.Delayed, null, expenditureSummary);

    /// <summary>
    /// Creates an acquisition result that failed because insufficient provisions were provided
    /// </summary>
    /// <param name="expenditureSummary">The provision expenditure for the acquireable</param>
    /// <returns>The result</returns>
    public static AcquisitionResult InsufficientProvisions(ProvisionExpenditureSummary expenditureSummary) => new(AcquisitionOutcome.InsufficientProvisions, null, expenditureSummary);

    /// <summary>
    /// Creates an acquisition result that failed because the max queue size was reached
    /// </summary>>
    /// <returns>The result</returns>
    public static AcquisitionResult MaxQueueSizeReached() => new(AcquisitionOutcome.MaxQueueSizeReached);

    /// <summary>
    /// Creates an acquisition result that failed because the max acquisitions allowed was reached
    /// </summary>
    /// <returns>The result</returns>
    public static AcquisitionResult MaxAcquisitionsReached() => new(AcquisitionOutcome.MaxAcquisitionsReached);

    #endregion

    #region Variables

    /// <summary>
    /// Describes if the acquisition result represents a successful acquisition
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Describes the nature of the outcome of the result
    /// </summary>
    public AcquisitionOutcome Outcome { get; }

    /// <summary>
    /// The expenditure summary for the acquireable
    /// </summary>
    public ProvisionExpenditureSummary ExpenditureSummary { get; }

    /// <summary>
    /// The acquireable that was acquired. Null if it was not immediately acquired
    /// </summary>
    public IAcquireable? Acquireable { get; }

    #endregion

    #region Constructors

    internal AcquisitionResult(AcquisitionOutcome outcome, IAcquireable? acquireable = null, ProvisionExpenditureSummary? summary = null)
    {
        if (outcome is AcquisitionOutcome.Success && acquireable is null)
        {
            throw new InvalidOperationException("Successful acquisition results must include the acquireable");
        }

        Outcome = outcome;
        ExpenditureSummary = summary ?? ProvisionExpenditureSummary.Empty;
        Acquireable = acquireable;
        Success = outcome is AcquisitionOutcome.Success || outcome is AcquisitionOutcome.Delayed;
    }

    #endregion
}
