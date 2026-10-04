using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

public interface IAcquisitionCostProvider
{
    IEnumerable<Provision> GetCost(HubAcquisitionContext context);
}
