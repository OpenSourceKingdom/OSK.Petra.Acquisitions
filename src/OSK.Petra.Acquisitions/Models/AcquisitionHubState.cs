using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionHubState
{
    public Guid HubId { get; init; }

    public Dictionary<Guid, AcquisitionHubChoiceState> ChoiceStates { get; init; } = [];
}
