using OSK.Petra.Acquisitions.Internal.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionRequirementContext
{
    #region Variables

    public IServiceProvider Services { get; }

    public IAcquirer Acquirer { get; }

    private readonly OwnerAcquisitionState _state;

    #endregion

    #region Constructors

    internal AcquisitionRequirementContext(IAcquirer acquirer, OwnerAcquisitionState ownerState, IServiceProvider serviceProvider)
    {
        Acquirer = acquirer ?? throw new ArgumentNullException(nameof(acquirer));
        _state = ownerState ?? throw new ArgumentNullException(nameof(ownerState));
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    #endregion

    #region Api

    public bool TryGetChoiceProgression(Guid pathAcquisitionId, Guid acquireableId, [NotNullWhen(true)] out AcquisitionTierProgression? tierProgression)
    {
        if (_state.AcquisitionPaths.TryGetValue(pathAcquisitionId, out var pathState))
        {
            tierProgression = new(acquireableId, pathState.ChoiceStates[acquireableId].TotalAcquisitions);
        }

        tierProgression = null;
        return false;
    }

    #endregion
}
