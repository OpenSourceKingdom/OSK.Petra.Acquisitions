using Moq;
using OSK.Petra.Acquisitions.Holds.Timers;
using OSK.Petra.Acquisitions.Models;

namespace OSK.Petra.Acquisitions.UnitTests.Holds.Timers;

public class TimerHoldTests
{
    #region RemainingTime (Get)

    [Fact]
    public void RemainingTime_NewTimer_SetsRemainingTimeToTimer()
    {
        // Arrange
        var hold = new TimerHold()
        {
            Timer = TimeSpan.FromSeconds(100)
        };

        // Act/Assert
        Assert.Equal(TimeSpan.FromSeconds(100), hold.RemainingTime);
    }

    #endregion

    #region GetPercentageComplete

    [Theory]
    [InlineData(100, 0, 0)]
    [InlineData(100, 25, .25)]
    [InlineData(100, 50, .50)]
    [InlineData(100, 75, .75)]
    [InlineData(100, 100, 1)]
    public void GetPercentageComplete_VaryingStartDeltaPercentageCases_ReturnsExecptedValues(double timerSeconds, double deltaSeconds, double expectedPercentage)
    {
        // Arrange
        var hold = new TimerHold()
        {
            Timer = TimeSpan.FromSeconds(timerSeconds),
            RemainingTime = TimeSpan.FromSeconds(timerSeconds) - TimeSpan.FromSeconds(deltaSeconds)
        };

        // Act/Assert
        Assert.Equal(expectedPercentage, hold.GetPercentageComplete());
    }

    #endregion

    #region Update

    [Fact]
    public void Update_TimerNotRan_SetsAndUpdatesIntenralTimer_ReturnsActiveHoldStatus ()
    {
        // Arrange
        var hold = new TimerHold()
        {
            Timer = TimeSpan.FromSeconds(100)
        };

        // Act
        var updateStatus = hold.Update(Mock.Of<IServiceProvider>(), TimeSpan.Zero);

        // Assert
        Assert.Equal(AcquisitionHoldStatus.Active, updateStatus);

    }

    [Fact]
    public void Update_TimerRan_DoesNotComplete_SetsAndUpdatesIntenralTimer_ReturnsActiveHoldStatus()
    {
        // Arrange
        var hold = new TimerHold()
        {
            Timer = TimeSpan.FromSeconds(100)
        };

        // Act
        var updateStatus = hold.Update(Mock.Of<IServiceProvider>(), TimeSpan.FromSeconds(99));

        // Assert
        Assert.Equal(AcquisitionHoldStatus.Active, updateStatus);
        Assert.Equal(.99, hold.GetPercentageComplete());
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public void Update_TimerRanToCompletion_SetsAndUpdatesIntenralTimer_ReturnsReleasedHoldStatus_TimeRemainingStaysZero(double deltaSeconds)
    {
        // Arrange
        var hold = new TimerHold()
        {
            Timer = TimeSpan.FromSeconds(100)
        };

        // Act
        var updateStatus = hold.Update(Mock.Of<IServiceProvider>(), TimeSpan.FromSeconds(deltaSeconds));

        // Assert
        Assert.Equal(AcquisitionHoldStatus.Released, updateStatus);
        Assert.Equal(TimeSpan.Zero, hold.RemainingTime);
        Assert.Equal(1, hold.GetPercentageComplete());
    }

    #endregion
}
