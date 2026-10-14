using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Internal.Services;

internal class AcquisitionOwnerHubManager(int ownerId, OwnerAcquisitionState ownerState, IServiceProvider serviceProvider)
{
    #region Variables

    private readonly Dictionary<Guid, AcquisitionHubGroup> _hubGroups = [];

    #endregion

    #region Api

    public int OwnerId => ownerId;

    public OwnerAcquisitionState OwnerState => ownerState;

    public int TotalGroups => _hubGroups.Count;

    public IAcquirerAcquisitionView GetAcquisitionView(IAcquisitionHub hub, IAcquirer acquirer)
    {
        AddAcquisitionHub(hub);

        return _hubGroups[hub.GroupId].GetAcquisitionView(hub, acquirer);
    }

    public void AddAcquisitionHub(IAcquisitionHub hub, AcquisitionHubState? hubState = null)
    {
        if (!_hubGroups.TryGetValue(hub.Id, out var group))
        {
            group = new(hub.GroupId, serviceProvider, ownerState);
            _hubGroups[hub.GroupId] = group;
        }

        group.AddHub(hub, hubState);
    }

    public void Update(TimeSpan deltaTime)
    {
        foreach (var hubGroup in _hubGroups.Values)
        {
            hubGroup.Update(deltaTime);
        }
    }

    public bool RemoveHub(IAcquisitionHub hub)
    {
        if (!_hubGroups.TryGetValue(hub.Id, out var group))
        {
            return false;
        }
        if (!group.RemoveHub(hub))
        {
            return false;
        }
        if (group.TotalHubs is 0)
        {
            _hubGroups.Remove(group.HubGroupId);
        }

        return true;
    }

    #endregion
}
