using OSK.Petra.Acquisitions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Options;

public class AcquisitionServiceOptions
{
    public IEnumerable<OwnerAcquisitionState> AcquisitionStates { get; init; } = [];
}