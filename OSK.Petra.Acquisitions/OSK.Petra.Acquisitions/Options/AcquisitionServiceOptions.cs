using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Acquisitions.Options;

public class AcquisitionServiceOptions
{
    public IEnumerable<OwnerAcquisitionState> AcquisitionStates { get; init; } = [];
}