using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

/// <summary>
/// Provides a way to determine the cost value of an acquisition process
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerOptional, HexagonalIntegrationType.IntegrationOptional)]
public interface IAcquisitionCostProvider
{
    /// <summary>
    /// Gets the cost of an acquisition, provided the given acquisition context
    /// </summary>
    /// <param name="context">The context providing insights to the acquisition process</param>
    /// <returns>The cost of the acquisition</returns>
    IEnumerable<Provision> GetCost(HubAcquisitionContext context);
}
