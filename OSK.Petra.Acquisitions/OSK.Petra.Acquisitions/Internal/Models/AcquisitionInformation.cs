using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OSK.Petra.Acquisitions.Internal.Models;

internal class AcquisitionInformation
{
    #region Constructors

    public AcquisitionInformation(IAcquisitionDefinition definition)
    {
        RootDefinition = definition ?? throw new ArgumentNullException(nameof(definition));
        
        ChoiceTiers = new()
        {
            { 0, [definition] }
        };
    }

    public AcquisitionInformation(IAcquisitionDefinition rootDefinition, AcquisitionPathType pathType, Dictionary<int, IReadOnlyCollection<IAcquisitionDefinition>> choiceTiers)
    {
        RootDefinition = rootDefinition ?? throw new ArgumentNullException(nameof(rootDefinition));
        PathType = pathType;
        ChoiceTiers = choiceTiers ?? throw new ArgumentNullException(nameof(choiceTiers));
    }

    #endregion

    #region Api

    public bool IsPath => PathType is not null;

    public IAcquisitionDefinition RootDefinition { get; }

    public Dictionary<int, IReadOnlyCollection<IAcquisitionDefinition>> ChoiceTiers { get; }

    [MemberNotNullWhen(true, nameof(IsPath))]
    public AcquisitionPathType? PathType { get; }

    public IAcquisitionDefinition? GetChoiceDefinition(Guid acquireableId)
        => ChoiceTiers.SelectMany(tier => tier.Value).FirstOrDefault(definition => definition.Acquireable.Id == acquireableId);

    public Acquisition ToAcquisition(AcquisitionHubGroup group, IAcquisitionHub hub, IAcquirer acquirer, AcquisitionPathState pathState)
        => IsPath
            ? new AcquisitionChoice(group, hub, acquirer, RootDefinition, RootDefinition, 0, pathState.RootChoice())
            : new AcquisitionPath(group, hub, acquirer, this, pathState);

    #endregion
}
