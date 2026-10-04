using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions;
using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionChoice(AcquisitionHubGroup hubGroup, IAcquisitionHub hub, IAcquirer acquirer, IAcquisitionDefinition pathAcquisition, IAcquisitionDefinition choiceDefinition, int tier, AcquisitionChoiceState choiceState) 
    : Acquisition, IAcquisitionChoice
{
    #region IAcquisitionChoice

    public IAcquireable Acquireable => choiceDefinition.Acquireable;

    public int Tier => tier;

    public int TotalQueued => choiceState.GetTotalPendingAcquisitions(hub.Id);

    public int TotalInProgress => choiceState.GetTotalAcquisitionsInProgress(hub.Id);

    public AcquisitionResult InitiateAcquisition(IEnumerable<Provision> provisions)
        => hubGroup.InitiateAcquisition(hub, acquirer, pathAcquisition.Acquireable, choiceDefinition.Acquireable, provisions);

    public IEnumerable<Provision> CancelAcquisition()
        => hubGroup.CancelAcquisition(hub, acquirer, pathAcquisition.Acquireable, choiceDefinition.Acquireable);

    public IEnumerable<AcquisitionProgress> GetProgresses()
        => choiceState.TryGetHubQueue(hub.Id, out var queue)
            ? queue.AcquisitionsInProgress.Select(holdState => new AcquisitionProgress(holdState.Hold.GetPercentageComplete()))
            : [];

    public bool IsAvailable { get; private set; } = AreRequirementsMet(choiceDefinition, acquirer, hubGroup.AcquisitionState, hubGroup.ServiceProvider);

    public ProvisionExpenditureSummary NextExpenditure { get; private set; } = GetExpenditureSummary(hubGroup, hub, acquirer, pathAcquisition, choiceDefinition, []);

    public bool IsComplete => choiceDefinition.Configuration.MaxAcquisitionCount.HasValue && choiceState.TotalAcquisitions >= choiceDefinition.Configuration.MaxAcquisitionCount;

    #endregion

    #region Api

    public void Update(IEnumerable<Provision> provisions)
    {
        IsAvailable = IsAvailable
            ? true
            : AreRequirementsMet(choiceDefinition, acquirer, hubGroup.AcquisitionState, hubGroup.ServiceProvider);

        NextExpenditure = GetExpenditureSummary(hubGroup, hub, acquirer, pathAcquisition, choiceDefinition, provisions);
    }

    #endregion

    #region Helpers

    private static ProvisionExpenditureSummary GetExpenditureSummary(AcquisitionHubGroup hubGroup, IAcquisitionHub hub, IAcquirer acquirer, IAcquisitionDefinition pathDefinition, IAcquisitionDefinition choiceDefinition, IEnumerable<Provision> provisions)
    {
        var context = hubGroup.GetHubAcquisitionContext(hub, acquirer, pathDefinition, choiceDefinition);
        var requiredProvisions = choiceDefinition.Configuration.AcquisitionCostProvider?.GetCost(context) ?? [];

        return ProvisionCalculator.CalculateExpenditure(requiredProvisions, provisions);
    }

    private static bool AreRequirementsMet(IAcquisitionDefinition choiceDefinition, IAcquirer acquirer, OwnerAcquisitionState acquisitionState, IServiceProvider serviceProvider)
    {
        var context = new AcquisitionRequirementContext(acquirer, acquisitionState, serviceProvider);

        return choiceDefinition.Configuration.Requirements.All(requirement => requirement.IsMet(context));
    }

    #endregion
}
