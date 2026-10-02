using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Options;
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

    public void Initialize(AcquisitionServiceOptions? options = null)
    {
        if (options?.AcquisitionStates is not null)
        {
            foreach (var state in options.AcquisitionStates)
            {
                _hubManagers[state.OwnerId] = new(state.OwnerId, state, serviceProvider);
            }
        }
    }

    public IEnumerable<OwnerAcquisitionState> GetStates()
        => _hubManagers.Values.Select(manager => manager.OwnerState);

    public IAcquisitionHubContext AddHub(IAcquisitionHub hub)
    {
        if (hub is null)
        {
            throw new ArgumentNullException(nameof(hub));
        }

        if (!_hubManagers.TryGetValue(hub.OwnerId, out var manager))
        {
            manager = new(hub.OwnerId, new() { OwnerId = hub.OwnerId }, serviceProvider);
            _hubManagers[hub.OwnerId] = manager;
        }

        return manager.GetOrAddHub(hub);
    }

    public IEnumerable<IAcquisitionHubContext> GetHubs(HubSearchOptions? searchOptions = null)
    {
        var managers = searchOptions?.OwnerIds is { Length :> 0 }
            ? searchOptions.Value.OwnerIds.Where(id => _hubManagers.TryGetValue(id, out _)).Select(id => _hubManagers[id])
            : _hubManagers.Values;

        return managers.SelectMany(manager => manager.GetHubContexts(searchOptions?.HubGroupIds));
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
