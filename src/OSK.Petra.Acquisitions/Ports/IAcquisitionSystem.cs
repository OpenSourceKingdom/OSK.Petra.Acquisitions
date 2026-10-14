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
    /// <summary>
    /// Adds a hub and its state to the acquisition system
    /// </summary>
    /// <param name="hub">The hub to add to the system</param>
    /// <param name="state">The related state that should be associated with the hub</param>
    void AddHub(IAcquisitionHub hub, AcquisitionHubState state);

    /// <summary>
    /// Generates a view for the hub with the given acquirer as the source of the view request.
    /// </summary>
    /// <param name="hub">The hub to get a view for</param>
    /// <param name="acquirer">The main acquirer the view request is generated for</param>
    /// <returns>The acquisition view for the hub tied to the acquirer requesting it</returns>
    /// /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>The provided hub will be automatically added, with an empty state, to the system if it is not currently registered.</item>
    /// <item>The acquirer may impact the availability, provision, and other requirements for a given view</item>
    /// </list>
    /// </remarks>
    IAcquirerAcquisitionView GetAcquisitionView(IAcquisitionHub hub, IAcquirer acquirer);

    /// <summary>
    /// Removes the hub and its related queue state from the system
    /// </summary>
    /// <param name="hub">The hub to remove</param>
    /// <returns>Whether the hub existed to be removed</returns>
    bool RemoveHub(IAcquisitionHub hub);

    /// <summary>
    /// Updates the acquisition system over a given delta
    /// </summary>
    /// <param name="deltaTime">The amount of time passed since last check</param>
    void Update(TimeSpan deltaTime);
}
