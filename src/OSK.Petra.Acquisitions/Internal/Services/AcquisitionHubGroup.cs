using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions;
using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Services;

internal class AcquisitionHubGroup
{
    #region Variables

    private bool _initialized = false;

    private readonly Guid _hubGroupId;
    private readonly Dictionary<Guid, (AcquisitionInformation AcquisitionInformation, AcquisitionPathState PathState)> _acquisitionInformationStatePairs = [];
    private readonly Dictionary<Guid, IAcquisitionHub> _hubs = [];

    #endregion

    #region Constructors

    public AcquisitionHubGroup(Guid hubGroupId, IServiceProvider serviceProvider, OwnerAcquisitionState ownerState)
    {
        _hubGroupId = hubGroupId;
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        AcquisitionState = ownerState ?? throw new ArgumentNullException(nameof(ownerState));
    }

    #endregion

    #region Api

    public Guid HubGroupId => _hubGroupId;

    public OwnerAcquisitionState AcquisitionState { get; }

    public IServiceProvider ServiceProvider { get; }

    public int TotalHubs => _hubs.Count;

    public HubAcquisitionContext GetHubAcquisitionContext(IAcquisitionHub hub, IAcquirer acquirer, IAcquisitionDefinition rootDefinition, IAcquisitionDefinition choiceDefinition)
    {
        var pathState = _acquisitionInformationStatePairs[rootDefinition.Acquireable.Id];
        var choiceState = pathState.PathState.ChoiceStates[choiceDefinition.Acquireable.Id];

        return new(hub, acquirer, choiceDefinition.Acquireable, choiceState.TotalAcquisitions, choiceState.TryGetHubQueue(hub.Id, out var hubQueue) ? hubQueue.TotalQueuedAcquisitions : 0, _hubs.Count, ServiceProvider);
    }

    public void AddHub(IAcquisitionHub hub, AcquisitionHubState? hubState = null)
    {
        if (_hubs.TryGetValue(hub.Id, out _))
        {
            return;
        }

        _hubs[hub.Id] = hub;
        if (!_initialized)
        {
            foreach (var acquisition in hub.GetAcquisitions())
            {
                _acquisitionInformationStatePairs[acquisition.Acquireable.Id] = (SharedAcquisitionPool.GetInformation(acquisition), AcquisitionState.GetOrAddChoiceState(acquisition.Acquireable.Id));
            }

            _initialized = true;
        }
        if (hubState is not null)
        {
            foreach (var choiceStateKvp in hubState.ChoiceStates)
            {
                if (!_acquisitionInformationStatePairs.TryGetValue(choiceStateKvp.Value.PathId, out var pathInformationPair))
                {
                    continue;
                }
                if (!pathInformationPair.PathState.ChoiceStates.TryGetValue(choiceStateKvp.Value.AcquireableId, out var choiceState))
                {
                    continue;
                }

                choiceState.TotalAcquisitions += choiceStateKvp.Value.TotalAcquisitions;
                choiceState.HubQueues[hub.Id] = choiceStateKvp.Value.QueueState;
            }
        }
    }

    public IAcquirerAcquisitionView GetAcquisitionView(IAcquisitionHub hub, IAcquirer acquirer)
    {
        AddHub(hub);

        return new AcquisitionView(hub, acquirer, _acquisitionInformationStatePairs.Select(kvp => SharedAcquisitionPool.GetAcquisition(this, hub, acquirer, kvp.Value.AcquisitionInformation, kvp.Value.AcquisitionInformation.Choices[kvp.Key].Item2, kvp.Value.PathState)));
    }

    public AcquisitionResult InitiateAcquisition(IAcquisitionHub hub, IAcquirer acquirer, IAcquireable pathAcquireable, IAcquireable selectedAcquireable, IEnumerable<Provision> provisions)
    {
        if (acquirer is null)
        {
            throw new ArgumentNullException(nameof(acquirer));
        }
        if (pathAcquireable is null)
        {
            throw new ArgumentNullException(nameof(pathAcquireable));
        }
        if (selectedAcquireable is null)
        {
            throw new ArgumentNullException(nameof(selectedAcquireable));
        }
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }

        var informationStatePair = _acquisitionInformationStatePairs[pathAcquireable.Id];
        var choiceState = informationStatePair.PathState.ChoiceStates[selectedAcquireable.Id];
        var choiceDefinition = informationStatePair.AcquisitionInformation.GetChoiceDefinition(selectedAcquireable.Id)!;

        if (IsMaxAcquisitionsReached(choiceState, choiceDefinition.Configuration))
        {
            return AcquisitionResult.MaxAcquisitionsReached();
        }

        var isQueued = false;
        if (IsConcurrencyLimitReached(hub, choiceState, choiceDefinition.Configuration))
        {
            if (IsQueueLimitReached(hub, choiceState, choiceDefinition.Configuration))
            {
                return AcquisitionResult.MaxQueueSizeReached();
            }

            isQueued = true;
        }

        var acquisitionContext = GetHubAcquisitionContext(hub, acquirer, informationStatePair.AcquisitionInformation.RootDefinition, choiceDefinition);
        var requiredProvisions = choiceDefinition.Configuration.AcquisitionCostProvider?.GetCost(acquisitionContext) ?? [];

        var provisionExpenditureSummmary = ProvisionCalculator.CalculateExpenditure(requiredProvisions, provisions);
        if (!provisionExpenditureSummmary.Sufficient)
        {
            return AcquisitionResult.InsufficientProvisions(provisionExpenditureSummmary);
        }

        var acquisitionHold = choiceDefinition.Configuration.AcquisitionHoldProvider?.GetHold(acquisitionContext);
        if (acquisitionHold is null)
        {
            CompleteAcquisition(hub, acquirer, informationStatePair.AcquisitionInformation, choiceDefinition, choiceState, AcquisitionHoldStatus.Released);
            return AcquisitionResult.Acquired(choiceDefinition.Acquireable, provisionExpenditureSummmary);
        }

        var hubQueue = choiceState.GetOrAddHubQueue(hub.Id);
        hubQueue.AddPendingAcquisition(new AcquisitionHoldState
        {
            Acquirer = acquirer,
            RequiredProvisions = requiredProvisions,
            Hold = acquisitionHold
        }, isQueued: isQueued);

        return AcquisitionResult.Delayed(provisionExpenditureSummmary);
    }

    public AcquisitionCancellationResult CancelAcquisition(IAcquisitionHub hub, IAcquirer acquirer, IAcquireable pathAcquireable, IAcquireable canceledAcquireable)
    {
        if (acquirer is null)
        {
            throw new ArgumentNullException(nameof(acquirer));
        }
        if (pathAcquireable is null)
        {
            throw new ArgumentNullException(nameof(pathAcquireable));
        }
        if (canceledAcquireable is null)
        {
            throw new ArgumentNullException(nameof(canceledAcquireable));
        }

        var informationStatePair = _acquisitionInformationStatePairs[pathAcquireable.Id];
        var choiceState = informationStatePair.PathState.ChoiceStates[canceledAcquireable.Id];

        if (!choiceState.TryGetHubQueue(hub.Id, out var hubQueue))
        {
            return new([]);
        }

        var removedHold = hubQueue.RemoveAcquisition(acquirer);
        if (hubQueue.TotalQueuedAcquisitions is 0)
        {
            choiceState.RemoveHubQueue(hub.Id);
        }

        return removedHold is null
            ? new ([])
            : new (removedHold.RequiredProvisions);

    }

    public bool RemoveHub(IAcquisitionHub hub)
    {
        if (!_hubs.Remove(hub.Id))
        {
            return false;
        }

        foreach (var choiceState in _acquisitionInformationStatePairs.Values.SelectMany(pair => pair.PathState.ChoiceStates.Values))
        {
            choiceState.RemoveHubQueue(hub.Id);
        }

        return true;
    }

    public void Update(TimeSpan deltaTime)
    {
        var choiceStatePairs = _acquisitionInformationStatePairs.SelectMany(pair 
            => pair.Value.PathState.ChoiceStates.Values.Select(state => new { AcquisitionInformation = pair.Value.AcquisitionInformation, ChoiceState = state, Choice = pair.Value.AcquisitionInformation.GetChoiceDefinition(state.AcquireableId) }))
                                                 .Where(pair => pair.ChoiceState is not null);

        foreach (var hub in _hubs.Values)
        {
            foreach (var pair in choiceStatePairs)
            {
                if (!pair.ChoiceState.TryGetHubQueue(hub.Id, out var hubQueue))
                {
                    continue;
                }

                while (hubQueue.PendingAcquisitions.Count > 0 && !IsConcurrencyLimitReached(hub, pair.ChoiceState, pair.Choice!.Configuration))
                {
                    hubQueue.StartNextAcquisition();
                }

                for (var index = hubQueue.AcquisitionsInProgress.Count; index > 0; index--)
                {
                    var acquisitionHoldState = hubQueue.AcquisitionsInProgress[index];
                    var holdStatus = acquisitionHoldState.Hold.Update(ServiceProvider, deltaTime);
                    if (holdStatus is not AcquisitionHoldStatus.Active)
                    {
                        hubQueue.AcquisitionsInProgress.RemoveAt(index);
                        CompleteAcquisition(hub, acquisitionHoldState.Acquirer, pair.AcquisitionInformation, pair.Choice!, pair.ChoiceState, holdStatus);
                    }
                }
            }
        }
    }

    #endregion

    #region Helpers

    private bool IsMaxAcquisitionsReached(AcquisitionChoiceState choiceState, AcquisitionChoiceConfiguration choiceConfiguration)
    {
        return choiceConfiguration.MaxAcquisitionCount.HasValue
            ? choiceState.TotalAcquisitions >= choiceConfiguration.MaxAcquisitionCount
            : false;
    }

    private bool IsConcurrencyLimitReached(IAcquisitionHub hub, AcquisitionChoiceState choiceState, AcquisitionChoiceConfiguration choiceConfiguration)
    {
        if (choiceState.HubQueues.TryGetValue(hub.Id, out var queue))
        {
            if (hub.Configuration.MaxConcurrentAcquisitions.HasValue && queue.AcquisitionsInProgress.Count >= hub.Configuration.MaxConcurrentAcquisitions.Value)
            {
                return true;
            }
            if (choiceConfiguration.MaxConcurrentAcquisitions.HasValue && queue.AcquisitionsInProgress.Count >= choiceConfiguration.MaxConcurrentAcquisitions.Value)
            {
                return true;
            }
        }

        return choiceConfiguration.MaxGroupConcurrentAcquisitions.HasValue
            ? choiceState.HubQueues.Sum(queue => queue.Value.AcquisitionsInProgress.Count) >= choiceConfiguration.MaxGroupConcurrentAcquisitions.Value
            : false;
    }

    private bool IsQueueLimitReached(IAcquisitionHub hub, AcquisitionChoiceState choiceState, AcquisitionChoiceConfiguration choiceConfiguration)
    {
        if (choiceState.HubQueues.TryGetValue(hub.Id, out var queue))
        {
            if (choiceConfiguration.MaxQueueSize.HasValue && queue.PendingAcquisitions.Count >= choiceConfiguration.MaxQueueSize.Value)
            {
                return true;
            }
        }

        return choiceConfiguration.MaxGroupQueueSize.HasValue
            ? choiceState.HubQueues.Sum(queue => queue.Value.PendingAcquisitions.Count) >= choiceConfiguration.MaxGroupQueueSize.Value
            : false;
    }

    private void CompleteAcquisition(IAcquisitionHub hub, IAcquirer acquirer, AcquisitionInformation acquisitionInformation, IAcquisitionDefinition choice, AcquisitionChoiceState state, AcquisitionHoldStatus holdStatus)
    {
        if (holdStatus is AcquisitionHoldStatus.Released)
        {
            state.TotalAcquisitions++;
        }

        var success = holdStatus is AcquisitionHoldStatus.Released;
        hub.ProcessAcquisition(new(acquirer, acquisitionInformation.PathType, acquisitionInformation.RootDefinition.Acquireable, choice.Acquireable, state.TotalAcquisitions, success, ServiceProvider));

        if (state.TryGetHubQueue(hub.Id, out var hubQueue))
        {
            hubQueue.StartNextAcquisition();
        }
    }

    #endregion
}
