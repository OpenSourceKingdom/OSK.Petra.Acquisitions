using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// A thing that can be acquired
/// </summary>
public interface IAcquireable
{
    /// <summary>
    /// The specific unique id for the thing being acquired
    /// </summary>
    Guid Id { get; }
}