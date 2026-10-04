using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

public class OwnerAcquisitionState
{
    #region Variables

    public required int OwnerId { get; init; }

    public Dictionary<Guid, AcquisitionPathState> AcquisitionChoices { get; } = [];

    #endregion

    #region Api

    public AcquisitionPathState GetOrAddChoiceState(Guid acquireableId)
    {
        if (!AcquisitionChoices.TryGetValue(acquireableId, out var choiceState))
        {
            choiceState = new AcquisitionPathState
            {
                RootChoiceId = acquireableId
            };
            AcquisitionChoices[acquireableId] = choiceState;
        }

        return choiceState;
    }

    #endregion
}
