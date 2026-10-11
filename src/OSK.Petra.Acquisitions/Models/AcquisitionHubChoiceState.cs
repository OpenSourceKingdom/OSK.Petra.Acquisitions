using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionHubChoiceState
{
    public required Guid PathId { get; init; }

    public required Guid AcquireableId { get; init; }

    public long TotalAcquisitions { get; init; }

    public required AcquisitionHubQueueState QueueState { get; init; }
}
