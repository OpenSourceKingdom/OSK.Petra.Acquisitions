using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;
using System;

namespace OSK.Petra.Acquisitions.Ports;

/// <summary>
/// The main acquisition system for consumers to use when interacting with the library
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerPointOfEntry)]
public interface IAcquisitionSystem
{
    void AddHub(IAcquisitionHub hub, AcquisitionHubState state);

    IAcquirerAcquisitionView GetAcquisitionView(IAcquisitionHub hub, IAcquirer acquirer);

    bool RemoveHub(IAcquisitionHub hub);

    void Update(TimeSpan deltaTime);
}
