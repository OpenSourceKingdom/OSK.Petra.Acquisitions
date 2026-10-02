using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionHubContext
{
    IAcquisitionHub Hub { get; }

    IEnumerable<AcquisitionChoiceInformation> GetChoices(IAcquirer acquirer);

    AcquisitionResult InitiateAcquisition(IAcquirer acquirer, IAcquireable acquireable, IEnumerable<Provision> provisions);

    IEnumerable<Provision> CancelAcquisition(IAcquirer acquirer, IAcquireable acquireable);
}