using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Acquisitions.Models;

public class OwnerAcquisitionState
{
    #region Variables

    public required int OwnerId { get; init; }

    public Dictionary<Guid, AcquisitionChoiceState> AcquisitionChoices { get; } = [];

    #endregion

    #region Api

    public AcquisitionChoiceState GetOrAddChoiceState(Guid acquireableId)
    {
        if (!AcquisitionChoices.TryGetValue(acquireableId, out var choiceState))
        {
            choiceState = new AcquisitionChoiceState
            {
                AcquireableId = acquireableId
            };
            AcquisitionChoices[acquireableId] = choiceState;
        }

        return choiceState;
    }

    #endregion
}
