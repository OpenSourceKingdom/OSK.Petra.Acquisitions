using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Internal.Services;

internal class AcquisitionView(IAcquisitionHub hub, IAcquirer acquirer, IReadOnlyCollection<Acquisition> acquisitions) : IAcquisitionView
{
    #region Variables

    private IEnumerable<Provision> _provisionCache = [];

    #endregion

    #region IAcquisitionView

    public IAcquisitionHub Hub => hub;

    public IAcquirer Acquirer => acquirer;

    public IReadOnlyCollection<IAcquisition> Acquisitions => acquisitions;

    public void Update(IEnumerable<Provision> provisions)
    {
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }

        _provisionCache = provisions;

        foreach (var acquisition in acquisitions)
        {
            acquisition.Update(_provisionCache);
        }
    }

    public void Refresh()
        => Update(_provisionCache);

    #endregion
}
