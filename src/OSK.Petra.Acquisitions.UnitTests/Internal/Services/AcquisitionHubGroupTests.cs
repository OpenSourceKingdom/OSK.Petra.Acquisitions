using Moq;
using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;

namespace OSK.Petra.Acquisitions.UnitTests.Internal.Services;

public class AcquisitionHubGroupTests
{
    #region Variables

    private readonly Guid _hubGroupId = Guid.NewGuid();
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly OwnerAcquisitionState _ownerState = new() { OwnerId = 1 };
    private readonly AcquisitionHubGroup _group;

    #endregion

    #region Constructors

    public AcquisitionHubGroupTests()
    {
        _serviceProvider = new();

        _group = new AcquisitionHubGroup(_hubGroupId, _serviceProvider.Object, _ownerState);
    }

    #endregion

    #region Constructors_Validation

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AcquisitionHubGroup(_hubGroupId, null!, _ownerState));
    }

    [Fact]
    public void Constructor_NullOwnerState_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AcquisitionHubGroup(_hubGroupId, _serviceProvider.Object, null!));
    }

    #endregion

    #region HubGroupId

    [Fact]
    public void HubGroupId_ReturnsCorrectId()
    {
        // Assert
        Assert.Equal(_hubGroupId, _group.HubGroupId);
    }

    #endregion

    #region AcquisitionState

    [Fact]
    public void AcquisitionState_ReturnsCorrectState()
    {
        // Assert
        Assert.Equal(_ownerState, _group.AcquisitionState);
    }

    #endregion

    #region ServiceProvider

    [Fact]
    public void ServiceProvider_ReturnsExpectedProvider()
    {
        // Assert
        Assert.Equal(_serviceProvider.Object, _group.ServiceProvider);
    }


    #endregion

    #region AddHub

    [Fact]
    public void AddHub_NewHub_NotInitialized_Initializes_IncrementsTotalHubs()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        hub.Setup(h => h.GetAcquisitions()).Returns(Enumerable.Empty<IAcquisitionDefinition>());

        // Act
        _group.AddHub(hub.Object);

        // Assert
        Assert.Equal(1, _group.TotalHubs);
        hub.Verify(m => m.GetAcquisitions(), Times.Once);
    }

    [Fact]
    public void AddHub_NewHub_Initialized_DoesNotInitialize_IncrementsTotalHubs()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        hub.Setup(h => h.GetAcquisitions()).Returns(Enumerable.Empty<IAcquisitionDefinition>());

        _group.AddHub(hub.Object);

        var hub2 = new Mock<IAcquisitionHub>();
        hub2.Setup(h => h.Id).Returns(Guid.NewGuid());

        // Act
        _group.AddHub(hub.Object);

        // Assert
        Assert.Equal(1, _group.TotalHubs);
        hub2.Verify(m => m.GetAcquisitions(), Times.Never);
    }

    [Fact]
    public void AddHub_DuplicateHub_DoesNotIncrementTotalHubs()
    {
        // Arrange
        var id = Guid.NewGuid();
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(id);
        hub.Setup(h => h.GetAcquisitions()).Returns(Enumerable.Empty<IAcquisitionDefinition>());

        _group.AddHub(hub.Object);

        // Act
        _group.AddHub(hub.Object);

        // Assert
        Assert.Equal(1, _group.TotalHubs);
    }

    #endregion

    #region RemoveHub

    [Fact]
    public void RemoveHub_ExistingHub_ReturnsTrue()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        hub.Setup(h => h.GetAcquisitions()).Returns(Enumerable.Empty<IAcquisitionDefinition>());
        _group.AddHub(hub.Object);

        // Act
        var result = _group.RemoveHub(hub.Object);

        // Assert
        Assert.True(result);
        Assert.Equal(0, _group.TotalHubs);
    }

    [Fact]
    public void RemoveHub_NonExistentHub_ReturnsFalse()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());

        // Act
        var result = _group.RemoveHub(hub.Object);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ValidDeltaTime_DoesNotThrow()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        hub.Setup(h => h.GetAcquisitions()).Returns(Enumerable.Empty<IAcquisitionDefinition>());
        _group.AddHub(hub.Object);

        // Act & Assert
        _group.Update(TimeSpan.FromSeconds(1));
    }

    #endregion
}
