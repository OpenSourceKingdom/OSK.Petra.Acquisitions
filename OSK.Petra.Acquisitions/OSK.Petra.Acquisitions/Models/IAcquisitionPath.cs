using OSK.Petra.Acquisitions.Options;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionPath: IAcquisition
{
    AcquisitionPathType PathType { get; }

    IAcquireable RootAcquireable { get; }

    int TotalTiers { get; }

    int TotalChoices { get; }

    IEnumerable<IAcquisitionChoice> GetChoices(AcquisitionPathSearchOptions? searchOptions = null);
}
