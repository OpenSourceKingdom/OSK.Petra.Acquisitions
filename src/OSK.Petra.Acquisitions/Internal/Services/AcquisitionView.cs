using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Internal.Services;

internal class AcquisitionView(IAcquisitionHub hub, IAcquirer acquirer, IReadOnlyCollection<Acquisition> acquisitions) : IAcquisitionView
{
    #region IAcquisitionView

    public IAcquisitionHub Hub => hub;

    public IAcquirer Acquirer => acquirer;

    public IReadOnlyCollection<IAcquisition> Acquisitions => acquisitions;

    public void Update(IEnumerable<Provision> provisions)
    {
        foreach (var acquisition in acquisitions)
        {
            acquisition.Update(provisions);
        }
    }

    #endregion
}
