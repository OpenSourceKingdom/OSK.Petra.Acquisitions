using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class OwnerAcquisitionState
{
    #region Variables

    public required int OwnerId { get; init; }

    public Dictionary<Guid, AcquisitionPathState> AcquisitionPaths { get; } = [];

    #endregion

    #region Api

    public AcquisitionPathState GetOrAddChoiceState(Guid acquireableId)
    {
        if (!AcquisitionPaths.TryGetValue(acquireableId, out var pathState))
        {
            pathState = new AcquisitionPathState
            {
                RootChoiceId = acquireableId
            };
            AcquisitionPaths[acquireableId] = pathState;
        }

        return pathState;
    }

    #endregion
}
