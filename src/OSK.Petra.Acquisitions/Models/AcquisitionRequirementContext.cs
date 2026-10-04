using System;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionRequirementContext(IAcquirer acquirer, OwnerAcquisitionState ownerState, IServiceProvider serviceProvider)
{
    #region Variables

    IServiceProvider Services { get; } = serviceProvider;

    public IAcquirer Acquirer { get; } = acquirer;

    public OwnerAcquisitionState OwnerState { get; } = ownerState;

    #endregion

    #region Api

    public bool TryGetChoiceProgression(Guid pathAcquisitionId, Guid acquireableId, [NotNullWhen(true)] out AcquisitionTierProgression? tierProgression)
    {
        if (OwnerState.AcquisitionChoices.TryGetValue(pathAcquisitionId, out var pathState))
        {
            tierProgression = new(acquireableId, pathState.ChoiceStates[acquireableId].TotalAcquisitions);
        }

        tierProgression = null;
        return false;
    }

    #endregion
}
