using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Describes the current state for a given <see cref="IAcquisitionHub"/>
/// </summary>
public class AcquisitionHubState
{
    /// <summary>
    /// The id of the unique hub this state is associated with
    /// </summary>
    public Guid HubId { get; init; }

    /// <summary>
    /// The states of hte individual choices within the hub
    /// </summary>
    public Dictionary<Guid, AcquisitionHubChoiceState> ChoiceStates { get; init; } = [];
}
