using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionCancellationResult
{
    #region Variables

    public IReadOnlyCollection<Provision> FreedProvisions { get; }

    #endregion

    #region Constructors

    internal AcquisitionCancellationResult(IEnumerable<Provision> freedProvisions)
    {
        FreedProvisions = freedProvisions?.ToArray() ?? throw new ArgumentNullException(nameof(freedProvisions));
    }

    #endregion
}
