using OSK.Hexagonal.MetaData;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Ports;

/// <summary>
/// A view that can be utilized for a UI to display a collection of acquisition choices for a user to select from
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IAcquisitionView
{
    /// <summary>
    /// The hub the acquisition view is associated with
    /// </summary>
    IAcquisitionHub Hub { get; }

    /// <summary>
    /// The acquirer that the acquisition view is associated with
    /// </summary>
    IAcquirer Acquirer { get; }

    /// <summary>
    /// The collection of acquisitions available at the time the view was generated
    /// </summary>
    IReadOnlyCollection<IAcquisition> Acquisitions { get; }

    /// <summary>
    /// Updates the internal availability related information based on continual provision change for a user
    /// </summary>
    /// <param name="provisions">The currently available provisions for choices within the view</param>
    void Update(IEnumerable<Provision> provisions);

    /// <summary>
    /// Updates the internal availability related information for potential changes.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This will utilize the previous provisions as the provision value</item>
    /// </list>
    /// </remarks>
    void Refresh();
}
