using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

/// <summary>
/// A hub is a focal point that an acquirer can acquire certain choices
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.ConsumerRequired)]
public interface IAcquisitionHub
{
    /// <summary>
    /// The id of the owner of the hub
    /// </summary>
    int OwnerId { get; }

    /// <summary>
    /// The unique id for this specific hub within the application
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The unique group the hub belongs
    /// </summary>
    Guid GroupId { get; }

    /// <summary>
    /// Hub specific configuration that impacts its related queue
    /// </summary>
    AcquisitionHubConfiguration Configuration { get; }

    /// <summary>
    /// Gets the acquisition options that will be presented to an acquirer when requested
    /// </summary>
    /// <returns>The various acquisitions that the hub offers acquirers</returns>
    IEnumerable<IAcquisitionDefinition> GetAcquisitions();

    /// <summary>
    /// Informs the hub of a completed acquisition and to process it within the application
    /// </summary>
    /// <param name="context">The context that provides insights on the completed acquisition process</param>
    void ProcessAcquisition(AcquisitionCompleteContext context);
}