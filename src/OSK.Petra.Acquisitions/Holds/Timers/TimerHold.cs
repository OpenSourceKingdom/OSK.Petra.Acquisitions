using OSK.Petra.Acquisitions.Models;
using System;

namespace OSK.Petra.Acquisitions.Holds.Timers;

/// <summary>
/// Represents a hold that utilizes a time span to prevent an acquisition process
/// </summary>
public class TimerHold : IAcquisitionHold
{
    #region Variables

    private TimeSpan? _remainingTime;

    #endregion

    #region Api

    /// <summary>
    /// The total time for the timer
    /// </summary>
    public TimeSpan Timer { get; init; }

    /// <summary>
    /// The remaining time before the timer ends
    /// </summary>
    public TimeSpan RemainingTime 
    { 
        get => _remainingTime ?? Timer;
        init => _remainingTime = value;
    }

    #endregion

    #region IAcquisitionHold

    public double GetPercentageComplete()
        => (Timer.TotalMilliseconds - (_remainingTime?.TotalMilliseconds ?? Timer.TotalMilliseconds)) / Timer.TotalMilliseconds;

    public AcquisitionHoldStatus Update(IServiceProvider serviceProvider, TimeSpan deltaTime)
    {
        if (_remainingTime is null)
        {
            _remainingTime = Timer;
        }

        if (RemainingTime <= TimeSpan.Zero)
        {
            return AcquisitionHoldStatus.Released;
        }

        _remainingTime -= deltaTime;
        _remainingTime = _remainingTime <= TimeSpan.Zero ? TimeSpan.Zero : _remainingTime;
        return _remainingTime <= TimeSpan.Zero
            ? AcquisitionHoldStatus.Released 
            : AcquisitionHoldStatus.Active;
    }

    #endregion
}
