using System;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionHold
{
    double GetPercentageComplete();

    AcquisitionHoldStatus Update(IServiceProvider serviceProvider, TimeSpan deltaTime);
}
