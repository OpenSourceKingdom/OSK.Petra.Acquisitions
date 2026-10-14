namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Provides hub specific configuration
/// </summary>
public class AcquisitionHubConfiguration
{
    /// <summary>
    /// The maximum amount of concurrent acquisitions the hub can perform
    /// </summary>
    public int? MaxConcurrentAcquisitions { get; init; } = 1;
}
