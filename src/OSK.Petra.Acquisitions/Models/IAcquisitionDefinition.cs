using System.Collections.Generic;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Defines an acquisition option that a hub can present to a user
/// </summary>
public interface IAcquisitionDefinition
{
    /// <summary>
    /// Specific configuration for this acquisition
    /// </summary>
    AcquisitionChoiceConfiguration Configuration { get; }

    /// <summary>
    /// The actual acquireable that will be acquired
    /// </summary>
    IAcquireable Acquireable { get; }

    /// <summary>
    /// The collection of choices that follows this choice in a path
    /// </summary>
    ICollection<IAcquisitionDefinition> NextChoices { get; }

    /// <summary>
    /// The collection of choices that precedes this choice in a path
    /// </summary>
    ICollection<IAcquisitionDefinition> PreviousChoices { get; }
}