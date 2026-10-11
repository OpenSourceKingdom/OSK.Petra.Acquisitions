using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionDefinition
{
    AcquisitionChoiceConfiguration Configuration { get; }

    IAcquireable Acquireable { get; }

    ICollection<IAcquisitionDefinition> NextChoices { get; }

    ICollection<IAcquisitionDefinition> PreviousChoices { get; }
}