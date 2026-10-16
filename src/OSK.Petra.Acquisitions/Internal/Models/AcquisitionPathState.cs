using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionPathState
{
    #region Variables

    public required Guid RootChoiceId { get; init; }

    public Dictionary<Guid, AcquisitionChoiceState> ChoiceStates { get; init; } = [];

    public HashSet<Guid> ActiveChoices { get; init; } = [];

    public HashSet<Guid> CompleteChoices { get; init; } = [];

    #endregion

    #region Api

    public AcquisitionChoiceState GetOrCreateState(Guid choiceId)
    {
        if (ChoiceStates.TryGetValue(choiceId, out var state))
        {
            return state;
        }

        state = new()
        {
            AcquireableId = choiceId
        };

        ChoiceStates[choiceId] = state;
        return state;
    }

    public bool TryGetChoiceState(Guid pathAcquireableId, [NotNullWhen(true)] out AcquisitionChoiceState? choiceState)
        => ChoiceStates.TryGetValue(pathAcquireableId, out choiceState);

    public AcquisitionChoiceState RootChoice()
        => ChoiceStates[RootChoiceId];

    #endregion 
}
