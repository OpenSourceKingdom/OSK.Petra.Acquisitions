using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Services;

internal class AcquisitionSystem(IServiceProvider serviceProvider) : IAcquisitionSystem
{
    #region Variables

    private readonly Dictionary<int, AcquisitionOwnerHubManager> _hubManagers = [];

    #endregion

    #region IAcquisitionSystem

    public void AddHub(IAcquisitionHub hub, AcquisitionHubState state)
    {
        if (hub is null)
        {
            throw new ArgumentNullException(nameof(hub));
        }

        if (!_hubManagers.TryGetValue(hub.OwnerId, out var manager))
        {
            manager = new AcquisitionOwnerHubManager(hub.OwnerId, new() { OwnerId = hub.OwnerId }, serviceProvider);
            _hubManagers[hub.OwnerId] = manager;
        }

        manager.AddAcquisitionHub(hub, state);
    }

    public IEnumerable<OwnerAcquisitionState> GetStates()
        => _hubManagers.Values.Select(manager => manager.OwnerState);

    public IAcquisitionView GetAcquisitionView(IAcquisitionHub hub, IAcquirer acquirer)
    {
        if (hub is null)
        {
            throw new ArgumentNullException(nameof(hub));
        }
        if (acquirer is null)
        {
            throw new ArgumentNullException(nameof(acquirer));
        }

        if (!_hubManagers.TryGetValue(hub.OwnerId, out var manager))
        {
            manager = new AcquisitionOwnerHubManager(hub.OwnerId, new() { OwnerId = hub.OwnerId }, serviceProvider);
            _hubManagers[hub.OwnerId] = manager;
        }

        return manager.GetAcquisitionView(hub, acquirer);
    }

    public bool RemoveHub(IAcquisitionHub hub)
    {
        if (hub is null)
        {
            throw new ArgumentNullException(nameof(hub));
        }

        if (!_hubManagers.TryGetValue(hub.OwnerId, out var manager))
        {
            return false;
        }

        var removed = manager.RemoveHub(hub);
        if (removed && manager.TotalGroups is 0)
        {
            _hubManagers.Remove(hub.OwnerId);
        }

        return removed;
    }

    public void Update(TimeSpan deltaTime)
    {
        foreach (var manager in _hubManagers.Values)
        {
            manager.Update(deltaTime);
        }
    }

    #endregion
}
