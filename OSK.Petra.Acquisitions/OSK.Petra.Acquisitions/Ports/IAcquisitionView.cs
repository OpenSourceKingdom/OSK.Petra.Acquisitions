using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

public interface IAcquirerAcquisitionView
{
    IAcquisitionHub Hub { get; }

    IAcquirer Acquirer { get; }

    IEnumerable<IAcquisition> Acquisitions { get; }

    void Update(IEnumerable<Provision> provisions);
}
