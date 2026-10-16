using Moq;
using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Acquisitions.UnitTests.Internal.Services;

public class AcquisitionViewTests
{
    #region Hub

    [Fact]
    public void Hub_ReturnsExpected()
    {
        // Arrange
        var mockHub = Mock.Of<IAcquisitionHub>();

        var view = new AcquisitionView(mockHub, Mock.Of<IAcquirer>(), []);

        // Act/Assert
        Assert.Equal(mockHub, view.Hub);
    }

    #endregion

    #region Acquirer

    [Fact]
    public void Acquirer_ReturnsExpected()
    {
        // Arrange
        var mockAcquirer = Mock.Of<IAcquirer>();

        var view = new AcquisitionView(Mock.Of<IAcquisitionHub>(), mockAcquirer, []);

        // Act/Assert
        Assert.Equal(mockAcquirer, view.Acquirer);
    }

    #endregion

    #region Acquisitions

    [Fact]
    public void Acquisitions_ReturnsExpected()
    {
        // Arrange
        IReadOnlyCollection<Acquisition> acquisitions = [];
        var view = new AcquisitionView(Mock.Of<IAcquisitionHub>(), Mock.Of<IAcquirer>(), acquisitions);

        // Act/Assert
        Assert.Equal(acquisitions, view.Acquisitions);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_NullProvisions_ThrowsArgumentNullException()
    {
        // Arrange
        var view = new AcquisitionView(Mock.Of<IAcquisitionHub>(), Mock.Of<IAcquirer>(), []);

        // Act/Assert
        Assert.Throws<ArgumentNullException>(() => view.Update(null!));
    }

    [Fact]
    public void Update_HasAcquisitions_CallsUpdate_ReturnsSuccessfully()
    {
        // Arrange
        var provisions = new List<Provision>();
        var mockAcquisition = new Mock<Acquisition>();
        IReadOnlyCollection<Acquisition> acquisitions = [mockAcquisition.Object];
        var view = new AcquisitionView(Mock.Of<IAcquisitionHub>(), Mock.Of<IAcquirer>(), acquisitions);

        // Act
        view.Update(provisions);

        // Assert
        mockAcquisition.Verify(m => m.Update(It.Is<IEnumerable<Provision>>(p => p == provisions)), Times.Once);
    }

    #endregion

    #region Refresh

    [Fact]
    public void Refresh_CalledWithoutUpdate_ReturnsSuccessfully()
    {
        // Arrange
        var mockAcquisition = new Mock<Acquisition>();
        IReadOnlyCollection<Acquisition> acquisitions = [mockAcquisition.Object];
        var view = new AcquisitionView(Mock.Of<IAcquisitionHub>(), Mock.Of<IAcquirer>(), acquisitions);

        // Act
        view.Refresh();

        // Assert
        mockAcquisition.Verify(m => m.Update(It.IsAny<IEnumerable<Provision>>()), Times.Once);
    }

    [Fact]
    public void Refresh_CalledAfterUpdate_UsesSameProvisionsAsUpdate_ReturnsSuccessfully()
    {
        // Arrange
        var provisions = new List<Provision>();
        var mockAcquisition = new Mock<Acquisition>();
        IReadOnlyCollection<Acquisition> acquisitions = [mockAcquisition.Object];
        var view = new AcquisitionView(Mock.Of<IAcquisitionHub>(), Mock.Of<IAcquirer>(), acquisitions);

        view.Update(provisions);
        mockAcquisition.Reset();

        // Act
        view.Refresh();

        // Assert
        mockAcquisition.Verify(m => m.Update(It.Is<IEnumerable<Provision>>(p => p == provisions)), Times.Once);
    }

    #endregion
}
