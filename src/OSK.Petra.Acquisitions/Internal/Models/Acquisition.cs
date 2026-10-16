using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal abstract class Acquisition: IAcquisition
{
    public abstract void Update(IEnumerable<Provision> provisions);
}
