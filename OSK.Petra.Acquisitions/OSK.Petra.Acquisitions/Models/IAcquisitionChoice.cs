using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionChoice
{
    AcquisitionChoiceConfiguration Configuration { get; }

    IAcquireable Acquireable { get; }

    IEnumerable<IAcquisitionChoice> NextChoices { get; }
}
