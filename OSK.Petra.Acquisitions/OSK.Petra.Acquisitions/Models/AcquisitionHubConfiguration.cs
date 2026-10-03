using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionHubConfiguration
{
    public int MaxConcurrentAcquisitions { get; init; } = 1;
}
