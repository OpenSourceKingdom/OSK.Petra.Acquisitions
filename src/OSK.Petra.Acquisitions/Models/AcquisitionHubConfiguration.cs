using OSK.Petra.Acquisitions.Ports;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Provides hub specific configuration
/// </summary>
public class AcquisitionHubConfiguration
{
    /// <summary>
    /// The total queue size for a backlog of acquisitions across a particular <see cref="IAcquisitionHub"/>
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>A null count represents an endless queue size</item>
    /// </list>
    /// </remarks>
    public int? MaxPendingAcquisitions { get; init; }

    /// <summary>
    /// The maximum amount of concurrent acquisitions the hub can perform
    /// </summary>
    public int? MaxConcurrentAcquisitions { get; init; } = 1;
}
