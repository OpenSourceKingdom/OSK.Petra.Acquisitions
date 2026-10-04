using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionChoice: IAcquisition
{
    IAcquireable Acquireable { get; }

    int Tier { get; }

    int TotalQueued { get; }

    int TotalInProgress { get; }

    bool IsAvailable { get; }

    bool IsComplete { get; }

    IEnumerable<AcquisitionProgress> GetProgresses();

    ProvisionExpenditureSummary NextExpenditure { get; }

    AcquisitionResult InitiateAcquisition(IEnumerable<Provision> provisions);

    IEnumerable<Provision> CancelAcquisition();
}
