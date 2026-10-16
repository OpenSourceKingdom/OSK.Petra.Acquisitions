using System;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Represents a hold that prevents an acquisition process from completing until it has released or blocked
/// </summary>
public interface IAcquisitionHold
{
    /// <summary>
    /// Gets the percentage representation of the hold's current progress to completion
    /// </summary>
    /// <returns>A value between 0-1 that signifies how close to completion the hold is</returns>
    double GetPercentageComplete();

    /// <summary>
    /// Updates the hold with a provided time delta
    /// </summary>
    /// <param name="serviceProvider">The related services for the hold</param>
    /// <param name="deltaTime">The time delta since the last iteration</param>
    /// <returns>A status for the hold</returns>
    AcquisitionHoldStatus Update(IServiceProvider serviceProvider, TimeSpan deltaTime);
}
