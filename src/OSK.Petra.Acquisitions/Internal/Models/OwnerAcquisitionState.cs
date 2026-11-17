using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class OwnerAcquisitionState
{
    #region Variables

    public required int OwnerId { get; init; }

    public Dictionary<Guid, AcquisitionPathState> AcquisitionPathStates { get; } = [];

    #endregion

    #region Api

    public AcquisitionPathState GetOrAddPathState(Guid acquireableId)
    {
        if (!AcquisitionPathStates.TryGetValue(acquireableId, out var pathState))
        {
            pathState = new AcquisitionPathState
            {
                RootChoiceId = acquireableId
            };
            AcquisitionPathStates[acquireableId] = pathState;
        }

        return pathState;
    }

    public bool TryGetPathState(Guid pathAcquireableId, [NotNullWhen(true)] out AcquisitionPathState? pathState)
        => AcquisitionPathStates.TryGetValue(pathAcquireableId, out pathState);

    #endregion
}
