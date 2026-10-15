using Moq;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using System.Net.NetworkInformation;

namespace OSK.Petra.Acquisitions.UnitTests.Internal.Services;

public class AcquisitionSystemTests
{
    #region Variables

    private readonly IServiceProvider _serviceProvider = new Mock<IServiceProvider>().Object;
    private readonly AcquisitionSystem _system;

    #endregion

    #region Constructors

    public AcquisitionSystemTests()
    {
        _system = new AcquisitionSystem(_serviceProvider);
    }

    #endregion

    #region AddHub

    [Fact]
    public void AddHub_NullHub_ThrowsArgumentNullException()
    {
        // Arrange
        var state = new AcquisitionHubState();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _system.AddHub(null!, state));
    }

    [Fact]
    public void AddHub_NullState_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _system.AddHub(Mock.Of<IAcquisitionHub>(), null!));
    }

    [Fact]
    public void AddHub_Valid_AddsSuccessfully()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.OwnerId).Returns(1);
        var state = new AcquisitionHubState();

        // Act
        _system.AddHub(hub.Object, state);

        // Assert
        var states = _system.GetStates();
        Assert.Single(states);
        Assert.Equal(1, states.First().OwnerId);
    }

    #endregion

    #region GetStates

    [Fact]
    public void GetStates_NoHubs_ReturnsEmpty()
    {
        // Act
        var states = _system.GetStates();

        // Assert
        Assert.Empty(states);
    }

    [Fact]
    public void GetStates_HubsWithState_ReturnsExpected()
    {
        // Arrange
        var definitions = new List<IAcquisitionDefinition>();

        var id = Guid.NewGuid();
        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(m => m.Acquireable.Id)
            .Returns(id);

        definitions.Add(mockDefinition.Object);

        var mockHub = new Mock<IAcquisitionHub>();
        mockHub.SetupGet(m => m.OwnerId)
            .Returns(1);
        mockHub.Setup(m => m.GetAcquisitions())
            .Returns(definitions);

        _system.AddHub(mockHub.Object, new());

        // Act
        var states = _system.GetStates();

        // Assert
        Assert.Single(states);
        Assert.Equal(1, states.First().OwnerId);
    }

    #endregion

    #region GetAcquisitionView

    [Fact]
    public void GetAcquisitionView_NullHub_ThrowsArgumentNullException()
    {
        // Arrange
        var acquirer = new Mock<IAcquirer>().Object;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _system.GetAcquisitionView(null!, acquirer));
    }

    [Fact]
    public void GetAcquisitionView_NullAcquirer_ThrowsArgumentNullException()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.OwnerId).Returns(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _system.GetAcquisitionView(hub.Object, null!));
    }

    [Fact]
    public void GetAcquisitionView_ValidInputs_ReturnsView()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.OwnerId)
            .Returns(1);
        var state = new AcquisitionHubState();
        _system.AddHub(hub.Object, state);

        // Act
        var view = _system.GetAcquisitionView(hub.Object, Mock.Of<IAcquirer>());

        // Assert
        Assert.NotNull(view);
    }

    #endregion

    #region RemoveHub

    [Fact]
    public void RemoveHub_NullHub_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _system.RemoveHub(null!));
    }

    [Fact]
    public void RemoveHub_NonExistentHub_ReturnsFalse()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.OwnerId).Returns(1);

        // Act
        var result = _system.RemoveHub(hub.Object);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveHub_ExistingHub_ReturnsTrueAndCleansUp()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.OwnerId).Returns(1);
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        var state = new AcquisitionHubState();
        _system.AddHub(hub.Object, state);

        // Act
        var result = _system.RemoveHub(hub.Object);

        // Assert
        Assert.True(result);
        Assert.Empty(_system.GetStates());
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ValidDeltaTime_DoesNotThrow()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.OwnerId).Returns(1);
        _system.AddHub(hub.Object, new AcquisitionHubState());

        // Act & Assert
        _system.Update(TimeSpan.FromSeconds(1));
    }

    #endregion
}
