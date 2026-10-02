using Microsoft.Extensions.DependencyInjection;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionHubGroup
{
    #region Variables

    private bool _initialized = false;

    private readonly Guid _hubGroupId;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Guid, ValueTuple<IAcquisitionChoice, AcquisitionChoiceState>> _choiceStates = [];
    private readonly Dictionary<Guid, AcquisitionHubLiaison> _contexts = [];

    #endregion

    #region Constructors

    public AcquisitionHubGroup(Guid hubGroupId, IServiceProvider serviceProvider)
    {
        _hubGroupId = hubGroupId;
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    #endregion

    #region Api

    public Guid HubGroupId => _hubGroupId;

    public int TotalHubs => _contexts.Count;

    public IEnumerable<IAcquisitionHubContext> Contexts => _contexts.Values;

    public IAcquisitionHubContext GetOrAddContext(IAcquisitionHub hub, OwnerAcquisitionState ownerAcquisitionState)
    {
        if (_contexts.TryGetValue(hub.Id, out var context))
        {
            return context;
        }
        if (!_initialized)
        {
            foreach (var choice in hub.GetChoiceAcquisitions())
            {
                _choiceStates[choice.Acquireable.Id] = (choice, ownerAcquisitionState.GetOrAddChoiceState(choice.Acquireable.Id));
            }

            _initialized = true;
        }

        context = (AcquisitionHubLiaison)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(AcquisitionHubLiaison), this, hub, _choiceStates.Values);
        _contexts[hub.Id] = context;

        return context;
    }

    public IAcquisitionHubContext? GetContext(Guid hubId)
        => _contexts.TryGetValue(hubId, out var context)
            ? context
            : null;

    public bool RemoveContext(IAcquisitionHub hub)
        => _contexts.Remove(hub.Id);

    public void UpdateHubs(TimeSpan deltaTime)
    {
        foreach (var context in _contexts.Values)
        {
            context.Update(deltaTime);
        }
    }

    public int GetGroupQueueSize(Guid acquireableId)
        => _contexts.Values.Sum(context => context.GetTotalPendingAcquisitions(acquireableId));

    public int GetGroupConcurrentAcquisitionCount(Guid acquireableId)
        => _contexts.Values.Sum(context => context.GetTotalConcurrentAcquisition(acquireableId));

    #endregion
}
