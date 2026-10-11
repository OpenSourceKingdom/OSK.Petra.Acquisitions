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

        Choices = new()
        {
            { definition.Acquireable.Id, ( 0, definition )}
        };

        MaxTier = 0;
    }

    public AcquisitionInformation(IAcquisitionDefinition rootDefinition, AcquisitionPathType pathType, IEnumerable<(int Tier, IAcquisitionDefinition Definition)> choices)
    {
        RootDefinition = rootDefinition ?? throw new ArgumentNullException(nameof(rootDefinition));
        PathType = pathType;
        Choices = choices?.ToDictionary(choiceTuple => choiceTuple.Definition.Acquireable.Id) ?? throw new ArgumentNullException(nameof(choices));

        MaxTier = Choices.Values.Max(choiceTuple => choiceTuple.Tier);
    }

    #endregion

    #region Api

    public int MaxTier { get; }

    public bool IsPath => PathType is not null;

    public IAcquisitionDefinition RootDefinition { get; }

    public Dictionary<Guid, (int Tier, IAcquisitionDefinition Definition)> Choices { get; }

    [MemberNotNullWhen(true, nameof(IsPath))]
    public AcquisitionPathType? PathType { get; }

    public IAcquisitionDefinition? GetChoiceDefinition(Guid acquireableId)
        => Choices.TryGetValue(acquireableId, out var choiceTuple)
            ? choiceTuple.Item2
            : null;

    #endregion
}
