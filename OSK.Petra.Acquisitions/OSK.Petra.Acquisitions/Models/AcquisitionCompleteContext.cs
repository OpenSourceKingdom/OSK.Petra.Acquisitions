using System;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Models;

public class AcquisitionCompleteContext(IAcquirer acquirer, IAcquireable? acquireable, long acquiredAmount, IServiceProvider services)
{
    public bool AcquisitionSuccessful { get; } = acquireable is not null;

    public IAcquirer Acquirer { get; } = acquirer;

    [MemberNotNullWhen(true, nameof(AcquisitionSuccessful))]
    public IAcquireable? Acquireable { get; } = acquireable;

    public long AcquiredAmount { get; } = acquiredAmount;

    public IServiceProvider Services { get; } = services;
}
