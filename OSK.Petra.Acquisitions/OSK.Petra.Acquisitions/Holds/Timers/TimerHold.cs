using OSK.Petra.Acquisitions.Models;
using System;

namespace OSK.Petra.Acquisitions.Holds.Timers;

public class TimerHold : IAcquisitionHold
{
    #region Variables

    private TimeSpan _remainingTime;

    #endregion

    #region Api

    public TimeSpan Timer { get; init; }

    public TimeSpan RemainingTime 
    { 
        get => _remainingTime;
        init => _remainingTime = value;
    }

    #endregion

    #region IAcquisitionHold

    public double GetPercentageComplete()
        => _remainingTime.TotalMilliseconds / Timer.TotalMilliseconds;

    public AcquisitionHoldStatus Update(IServiceProvider serviceProvider, TimeSpan deltaTime)
    {
        if (_remainingTime >= Timer)
        {
            return AcquisitionHoldStatus.Released;
        }

        _remainingTime += deltaTime;
        _remainingTime = _remainingTime >= Timer ? Timer : _remainingTime;
        return _remainingTime >= Timer 
            ? AcquisitionHoldStatus.Released 
            : AcquisitionHoldStatus.Active;
    }

    #endregion
}
