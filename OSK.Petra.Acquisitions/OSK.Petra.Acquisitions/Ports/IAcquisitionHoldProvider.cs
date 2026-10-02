using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;

namespace OSK.Petra.Acquisitions.Ports;

/// <summary>
/// Provides acquisition holds that will prevent an acquisition process
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerOptional, HexagonalIntegrationType.IntegrationOptional)]
public interface IAcquisitionHoldProvider
{
    /// <summary>
    /// Determines if an acquisition exists for the current context that would prevent the acquisition process from continuing
    /// </summary>
    /// <param name="context">The acquisition context to get a hold for</param>
    /// <returns>The acquisition hold, if one is needed</returns>
    IAcquisitionHold? GetHold(AcquisitionProcessContext context);
}
