using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Options;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

/// <summary>
/// The main acquisition system for consumers to use when interacting with the library
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerPointOfEntry)]
public interface IAcquisitionSystem
{
    void Initialize(AcquisitionServiceOptions? options = null);

    IEnumerable<OwnerAcquisitionState> GetStates();

    IAcquirerAcquisitionView GetAcquisitionView(IAcquisitionHub hub, IAcquirer acquirer);

    bool RemoveHub(IAcquisitionHub hub);

    void Update(TimeSpan deltaTime);
}
