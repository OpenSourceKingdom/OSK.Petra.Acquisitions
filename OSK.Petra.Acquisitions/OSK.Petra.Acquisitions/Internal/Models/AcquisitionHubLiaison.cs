using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions;
using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionHubLiaison : IAcquisitionHubLiaison
{
    #region Variables

    private readonly IAcquisitionHub _hub;
    private readonly AcquisitionHubGroup _group;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<Guid, (IAcquisitionChoice Choice, AcquisitionChoiceState State)> _choiceStates;

    #endregion

    #region Constructors

    public AcquisitionHubLiaison(AcquisitionHubGroup hubGroup, IAcquisitionHub hub, IServiceProvider services, IEnumerable<ValueTuple<IAcquisitionChoice, AcquisitionChoiceState>> choiceStates)
    {
        _group = hubGroup ?? throw new ArgumentNullException(nameof(hubGroup));
        _hub = hub ?? throw new ArgumentNullException(nameof(hub));
        _serviceProvider = services ?? throw new ArgumentNullException(nameof(services));

        _choiceStates = choiceStates?.ToDictionary(s => s.Item1.Acquireable.Id) ?? [];
    }

    #endregion

    #region IAcquisitionHubContext

    public IAcquisitionHub Hub => _hub;

    public AcquisitionResult InitiateAcquisition(IAcquirer acquirer, IAcquireable acquireable, IEnumerable<Provision> provisions)
    {
        if (acquirer is null)
        {
            throw new ArgumentNullException(nameof(acquirer));
        }
        if (acquireable is null)
        {
            throw new ArgumentNullException(nameof(acquireable));
        }
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }

        if (!_choiceStates.TryGetValue(acquireable.Id, out var choiceStatePair))
        {
            return AcquisitionResult.InvalidAcquireable();
        }
        if (IsMaxAcquisitionsReached(choiceStatePair.State, choiceStatePair.Choice.Configuration))
        {
            return AcquisitionResult.MaxAcquisitionsReached();
        }

        var isQueued = false;
        if (IsConcurrencyLimitReached(choiceStatePair.State, choiceStatePair.Choice.Configuration))
        {
            if (IsQueueLimitReached(choiceStatePair.State, choiceStatePair.Choice.Configuration))
            {
                return AcquisitionResult.MaxQueueSizeReached();
            }

            isQueued = true;
        }

        var acquisitionContext = GetAcquisitionContext(acquirer, choiceStatePair);
        var requiredProvisions = choiceStatePair.Choice.Configuration.AcquisitionCostProvider?.GetCost(acquisitionContext) ?? [];

        var provisionExpenditureSummmary = ProvisionCalculator.CalculateExpenditure(requiredProvisions, provisions);
        if (!provisionExpenditureSummmary.Sufficient)
        {
            return AcquisitionResult.InsufficientProvisions(provisionExpenditureSummmary);
        }

        var acquisitionHold = choiceStatePair.Choice.Configuration.AcquisitionHoldProvider?.GetHold(acquisitionContext);
        if (acquisitionHold is null)
        {
            CompleteAcquisition(acquirer, choiceStatePair.Choice, choiceStatePair.State, AcquisitionHoldStatus.Released);
            return AcquisitionResult.Acquired(choiceStatePair.Choice.Acquireable, provisionExpenditureSummmary);
        }

        var hubQueue = choiceStatePair.State.GetOrAddHubQueue(_hub.Id);
        hubQueue.AddPendingAcquisition(new AcquisitionHoldState
        {
            Acquirer = acquirer,
            RequiredProvisions = requiredProvisions,
            Hold = acquisitionHold
        }, isQueued: isQueued);

        return AcquisitionResult.Delayed(provisionExpenditureSummmary);
    }

    public IEnumerable<Provision> CancelAcquisition(IAcquirer acquirer, IAcquireable acquireable)
    {
        if (!_choiceStates.TryGetValue(acquireable.Id, out var choiceStatePair))
        {
            return [];
        }
        if (!choiceStatePair.State.TryGetHubQueue(_hub.Id, out var hubQueue))
        {
            return [];
        }

        var removedHold = hubQueue.RemoveAcquisition(acquirer);
        if (hubQueue.TotalQueuedAcquisitions is 0)
        {
            choiceStatePair.State.RemoveHubQueue(_hub.Id);
        }

        return removedHold is null
            ? []
            : removedHold.RequiredProvisions;

    }

    public AcquirerChoiceContext GetChoices(IAcquirer acquirer)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Api

    public int GetTotalPendingAcquisitions(Guid acquireableId)
        => _choiceStates.TryGetValue(acquireableId, out var choiceStatePair) && choiceStatePair.State.TryGetHubQueue(_hub.Id, out var hubQueue)
            ? hubQueue.PendingAcquisitions.Count
            : 0;

    public int GetTotalConcurrentAcquisition(Guid acquireableId)
        => _choiceStates.TryGetValue(acquireableId, out var choiceStatePair) && choiceStatePair.State.TryGetHubQueue(_hub.Id, out var hubQueue)
            ? hubQueue.AcquisitionsInProgress.Count
            : 0;

    public void Update(TimeSpan deltaTime)
    {
        foreach (var choiceStatePair in _choiceStates.Values)
        {
            if (!choiceStatePair.State.TryGetHubQueue(_hub.Id, out var hubQueue))
            {
                continue;
            }

            while (hubQueue.PendingAcquisitions.Count > 0 && !IsConcurrencyLimitReached(choiceStatePair.State, choiceStatePair.Choice.Configuration))
            {
                hubQueue.StartNextAcquisition();
            }

            for (var index = hubQueue.AcquisitionsInProgress.Count; index > 0; index--)
            {
                var acquisitionHoldState = hubQueue.AcquisitionsInProgress[index];
                var holdStatus = acquisitionHoldState.Hold.Update(_serviceProvider, deltaTime);
                if (holdStatus is not AcquisitionHoldStatus.Active)
                {
                    hubQueue.AcquisitionsInProgress.RemoveAt(index);
                    CompleteAcquisition(acquisitionHoldState.Acquirer, choiceStatePair.Choice, choiceStatePair.State, holdStatus);
                }
            }
        }
    }

    #endregion

    #region Helpers

    private void CompleteAcquisition(IAcquirer acquirer, IAcquisitionChoice choice, AcquisitionChoiceState state, AcquisitionHoldStatus holdStatus)
    {
        if (holdStatus is AcquisitionHoldStatus.Released)
        {
            state.TotalAcquisitions++;
        }

        _hub.ProcessAcquisition(new(acquirer, holdStatus is AcquisitionHoldStatus.Released ? choice.Acquireable : null, state.TotalAcquisitions, _serviceProvider));

        if (state.TryGetHubQueue(_hub.Id, out var hubQueue))
        {
            hubQueue.StartNextAcquisition();
        }
    }

    private bool IsMaxAcquisitionsReached(AcquisitionChoiceState state, AcquisitionChoiceConfiguration configuration)
    {
        return configuration.MaxAcquisitionCount.HasValue
            ? state.TotalAcquisitions >= configuration.MaxAcquisitionCount
            : false;
    }

    private bool IsConcurrencyLimitReached(AcquisitionChoiceState state, AcquisitionChoiceConfiguration configuration)
    {
        if (configuration.MaxGroupConcurrentAcquisitions.HasValue && _group.GetGroupConcurrentAcquisitionCount(state.AcquireableId) >= configuration.MaxGroupConcurrentAcquisitions.Value)
        {
            return true;
        }

        return configuration.MaxConcurrentAcquisitions.HasValue
            ? GetTotalConcurrentAcquisition(state.AcquireableId) >= configuration.MaxConcurrentAcquisitions.Value
            : false;
    }

    private bool IsQueueLimitReached(AcquisitionChoiceState state, AcquisitionChoiceConfiguration configuration)
    {
        if (configuration.MaxGroupQueueSize.HasValue && _group.GetGroupQueueSize(state.AcquireableId) >= configuration.MaxGroupQueueSize.Value)
        {
            return true;
        }

        return configuration.MaxQueueSize.HasValue
            ? GetTotalPendingAcquisitions(state.AcquireableId) >= configuration.MaxQueueSize.Value
            : false;
    }

    private AcquisitionProcessContext GetAcquisitionContext(IAcquirer acquirer, ValueTuple<IAcquisitionChoice, AcquisitionChoiceState> choiceStatePair)
        => new(acquirer, choiceStatePair.Item1.Acquireable, choiceStatePair.Item2.TotalAcquisitions, choiceStatePair.Item2.TryGetHubQueue(_hub.Id, out var hubQueue) ? hubQueue.TotalQueuedAcquisitions : 0, _group.TotalHubs, _serviceProvider);

    #endregion
}
