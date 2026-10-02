using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionOwnerHubManager(int ownerId, OwnerAcquisitionState ownerState, IServiceProvider serviceProvider)
{
    #region Variables

    private readonly Dictionary<Guid, AcquisitionHubGroup> _hubGroups = [];

    #endregion

    #region Api

    public int OwnerId => ownerId;

    public OwnerAcquisitionState OwnerState => ownerState;

    public int TotalGroups => _hubGroups.Count;

    public void Update(TimeSpan deltaTime)
    {
        foreach (var hubGroup in _hubGroups.Values)
        {
            hubGroup.UpdateHubs(deltaTime);
        }
    }

    public IEnumerable<IAcquisitionHubContext> GetHubContexts(Guid[]? hubIdFilter)
    {
        return hubIdFilter is { Length: > 0 }
            ? hubIdFilter.Where(groupId => _hubGroups.TryGetValue(groupId, out _)).SelectMany(groupId => _hubGroups[groupId].Contexts)
            : _hubGroups.Values.SelectMany(group => group.Contexts);
    }

    public IAcquisitionHubContext GetOrAddHub(IAcquisitionHub hub)
    {
        if (!_hubGroups.TryGetValue(hub.GroupId, out var hubGroup))
        {
            hubGroup = new AcquisitionHubGroup(hub.GroupId, serviceProvider);
            _hubGroups[hub.GroupId] = hubGroup;
        }

        return hubGroup.GetOrAddContext(hub, ownerState);
    }

    public bool RemoveHub(IAcquisitionHub hub)
    {
        if (!_hubGroups.TryGetValue(hub.Id, out var group))
        {
            return false;
        }
        if (!group.RemoveContext(hub))
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
