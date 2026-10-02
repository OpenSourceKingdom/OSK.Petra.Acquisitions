using OSK.Petra.Provisions.Models;
using System;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionResult
{
    #region Static

    public static AcquisitionResult Acquired(IAcquireable acquireable, ProvisionExpenditureSummary? expenditureSummary = null) => new(AcquisitionOutcome.Success, acquireable, expenditureSummary);

    public static AcquisitionResult Delayed(ProvisionExpenditureSummary? expenditureSummary = null) => new(AcquisitionOutcome.Delayed, null, expenditureSummary);

    public static AcquisitionResult InsufficientProvisions(ProvisionExpenditureSummary expenditureSummary) => new(AcquisitionOutcome.InsufficientProvisions, null, expenditureSummary);

    public static AcquisitionResult MaxQueueSizeReached() => new(AcquisitionOutcome.MaxQueueSizeReached);
    public static AcquisitionResult MaxAcquisitionsReached() => new(AcquisitionOutcome.MaxAcquisitionsReached);
    public static AcquisitionResult InvalidAcquireable() => new(AcquisitionOutcome.InvalidAcquireable);

    #endregion

    #region Variables

    public bool Success { get; }

    public AcquisitionOutcome Outcome { get; }

    public ProvisionExpenditureSummary ExpenditureSummary { get; }

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
