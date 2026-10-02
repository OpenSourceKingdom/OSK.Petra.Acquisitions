using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

[HexagonalIntegration(HexagonalIntegrationType.ConsumerRequired)]
public interface IAcquisitionHub
{
    int OwnerId { get; }

    Guid Id { get; }

    Guid GroupId { get; }

    AcquisitionHubConfiguration Configuration { get; }

    IEnumerable<IAcquisitionChoice> GetChoiceAcquisitions();

    void ProcessAcquisition(AcquisitionCompleteContext context);
}