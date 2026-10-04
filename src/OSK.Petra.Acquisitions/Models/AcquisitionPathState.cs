using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionPathState
{
    #region Variables

    public required Guid RootChoiceId { get; init; }

    public Dictionary<Guid, AcquisitionChoiceState> ChoiceStates { get; init; } = [];

    public HashSet<Guid> ActiveChoices { get; init; } = [];

    public HashSet<Guid> CompleteChoices { get; init; } = [];

    #endregion

    #region Api

    public AcquisitionChoiceState RootChoice()
        => ChoiceStates[RootChoiceId];

    #endregion 
}
