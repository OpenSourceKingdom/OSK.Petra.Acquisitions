using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// The entity attempting to acquisition a thing in the system. Can represent an entire entity (e.g. player) or a part (e.g. individual character)
/// </summary>
public interface IAcquirer
{
    /// <summary>
    /// The id of the owner for this acquireer in the application
    /// </summary>
    int OwnerId { get; }
    
    /// <summary>
    /// The id of the team the acquirerer belongs to
    /// </summary>
    int TeamId { get; }

    /// <summary>
    /// An id that represents the specific character for an acquisition process
    /// </summary>
    Guid? Id { get; }
}
