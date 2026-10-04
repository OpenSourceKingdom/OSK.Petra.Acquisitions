using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionHoldState
{
    #region Variables

    public required IAcquirer Acquirer { get; init; }

    public IEnumerable<Provision> RequiredProvisions { get; init; } = [];

    public required IAcquisitionHold Hold { get; init; }

    #endregion
}
