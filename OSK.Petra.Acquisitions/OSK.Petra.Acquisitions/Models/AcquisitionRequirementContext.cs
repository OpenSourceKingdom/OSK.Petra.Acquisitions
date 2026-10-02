using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionRequirementContext(IAcquirer acquirer, OwnerAcquisitionState ownerState)
{
    #region Variables

    public IAcquirer Acquirer { get; } = acquirer;

    public OwnerAcquisitionState OwnerState { get; } = ownerState;

    #endregion

    #region Api

    public bool TryGetChoice(Guid acquireableId, [NotNullWhen(true)] out AcquisitionChoiceState? choiceState)
        => OwnerState.AcquisitionChoices.TryGetValue(acquireableId, out choiceState);

    #endregion
}
