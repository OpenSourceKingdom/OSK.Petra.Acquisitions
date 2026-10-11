using OSK.Petra.Acquisitions.Ports;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Provides settings for how a particular <see cref="IAcquireable"/> will process acquisitions
/// </summary>
/// <remarks>
/// 💡Notes:
/// <list type="bullet">
/// <item>For cases where global configurations are set, the first acquireable configuration provided for a particular <see cref="IAcquireable"/> will be preferred</item>
/// </list>
/// </remarks>
public class AcquisitionChoiceConfiguration
{
    /// <summary>
    /// The collection of requirements that must be met to consider the <see cref="IAcquireable"/> as available
    /// </summary>
    public IEnumerable<IAcquisitionRequirement> Requirements { get; init; } = [];

    /// <summary>
    /// The total amount of acquisitions an <see cref="IAcquirer"/> can make for the associated <see cref="IAcquireable"/>
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>A null count represents an endless acquisition amount</item>
    /// </list>
    /// </remarks>
    public int? MaxAcquisitionCount { get; init; }

    /// <summary>
    /// The total number of acquisitions that can be acquired across an individual <see cref="IAcquisitionHub"/>
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>A null count represents an endless amount of concurrent acquisition processes</item>
    /// </list>
    /// </remarks>
    public int? MaxConcurrentAcquisitions { get; init; }

    /// <summary>
    /// The total number of acquisitions that can be acquired across a particular <see cref="IAcquisitionHub.GroupId"/>
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>A null count represents an endless amount of concurrent acquisition processes across the group</item>
    /// </list>
    /// </remarks>
    public int? MaxGroupConcurrentAcquisitions { get; init; }

    /// <summary>
    /// The total queue size for a backlog of acquisitions across a particular <see cref="IAcquisitionHub"/>
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>A null count represents an endless queue size</item>
    /// </list>
    /// </remarks>
    public int? MaxQueueSize { get; init; }

    /// <summary>
    /// The total queue size for a backlog of acquisitions across a particular <see cref="IAcquisitionHub.GroupId"/>
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>A null count represents an endless queue size across the group</item>
    /// </list>
    /// </remarks>
    public int? MaxGroupQueueSize { get; init; }

    /// <summary>
    /// A provider that sets the hold for a current acquisition process (e.g. a timer hold to wait x seconds prior to acquiring')
    /// </summary>
    public IAcquisitionHoldProvider? AcquisitionHoldProvider { get; init; }

    /// <summary>
    /// A provider that gets the cost of a given acquisition
    /// </summary>
    public IAcquisitionCostProvider? AcquisitionCostProvider { get; init; }
}
