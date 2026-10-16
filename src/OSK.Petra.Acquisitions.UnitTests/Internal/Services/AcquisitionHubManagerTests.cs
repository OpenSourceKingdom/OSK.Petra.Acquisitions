using Moq;
using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;

namespace OSK.Petra.Acquisitions.UnitTests.Internal.Services;

public class AcquisitionHubManagerTests
{
    #region Variables

    private readonly int _ownerId = 1;
    private readonly OwnerAcquisitionState _ownerState = new() { OwnerId = 1 };
    private readonly IServiceProvider _serviceProvider = new Mock<IServiceProvider>().Object;
    private readonly AcquisitionOwnerHubManager _manager;

    #endregion

    #region Constructors

    public AcquisitionHubManagerTests()
    {
        _manager = new AcquisitionOwnerHubManager(_ownerId, _ownerState, _serviceProvider);
    }

    #endregion

    #region OwnerId

    [Fact]
    public void OwnerId_ReturnsCorrectId()
    {
        // Arrange/Act/Assert
        Assert.Equal(_ownerId, _manager.OwnerId);
    }

    #endregion

    #region OwnerState

    [Fact]
    public void OwnerState_ReturnsCorrectState()
    {
        // Arrange/Act/Assert
        Assert.Equal(_ownerState, _manager.OwnerState);
    }

    #endregion

    #region TotalGroups

    [Fact]
    public void TotalGroups_NoGroups_ReturnsZero()
    {
        // Arrange/Act/Assert
        Assert.Equal(0, _manager.TotalGroups);
    }


    [Fact]
    public void TotalGroups_HasGroups_ReturnsExpectedCount()
    {
        // Arrange
        _manager.AddAcquisitionHub(Mock.Of<IAcquisitionHub>());

        // Act/Assert
        Assert.Equal(1, _manager.TotalGroups);
    }

    #endregion

    #region AddAcquisitionHub

    [Fact]
    public void AddAcquisitionHub_NewGroup_IncrementsTotalGroups()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.GroupId).Returns(Guid.NewGuid());
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());

        // Act
        _manager.AddAcquisitionHub(hub.Object);

        // Assert
        Assert.Equal(1, _manager.TotalGroups);
    }

    [Fact]
    public void AddAcquisitionHub_ExistingGroup_DoesNotIncrementTotalGroups()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var hub1 = new Mock<IAcquisitionHub>();
        hub1.Setup(h => h.GroupId).Returns(groupId);
        hub1.Setup(h => h.Id).Returns(Guid.NewGuid());

        var hub2 = new Mock<IAcquisitionHub>();
        hub2.Setup(h => h.GroupId).Returns(groupId);
        hub2.Setup(h => h.Id).Returns(Guid.NewGuid());

        _manager.AddAcquisitionHub(hub1.Object);

        // Act
        _manager.AddAcquisitionHub(hub2.Object);

        // Assert
        Assert.Equal(1, _manager.TotalGroups);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_CallsAllGroups()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.GroupId).Returns(Guid.NewGuid());
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        _manager.AddAcquisitionHub(hub.Object);

        var deltaTime = TimeSpan.FromSeconds(1);

        // Act & Assert
        _manager.Update(deltaTime);
    }

    #endregion

    #region RemoveHub

    [Fact]
    public void RemoveHub_NonExistentHub_ReturnsFalse()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());

        _manager.AddAcquisitionHub(hub.Object);

        // Act
        var result = _manager.RemoveHub(Mock.Of<IAcquisitionHub>());

        // Assert
        Assert.False(result);
        Assert.Equal(1, _manager.TotalGroups);
    }

    [Fact]
    public void RemoveHub_ExistingHub_GroupGreaterThanOne_GroupIsNotRemoved_ReturnsTrue()
    {
        // Arrange
        var groupId = Guid.NewGuid();

        var id1 = Guid.NewGuid();
        var hub1 = new Mock<IAcquisitionHub>();
        hub1.Setup(h => h.Id).Returns(id1);
        hub1.Setup(h => h.GroupId).Returns(groupId);

        var id2 = Guid.NewGuid();
        var hub2 = new Mock<IAcquisitionHub>();
        hub2.Setup(h => h.Id).Returns(id2);
        hub2.Setup(h => h.GroupId).Returns(groupId);

        _manager.AddAcquisitionHub(hub1.Object);
        _manager.AddAcquisitionHub(hub2.Object);

        // Act
        var result = _manager.RemoveHub(hub1.Object);

        // Assert
        Assert.True(result);
        Assert.Equal(1, _manager.TotalGroups);
    }

    [Fact]
    public void RemoveHub_ExistingHub_GroupEqualsZero_GroupIsRemoved_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var hub1 = new Mock<IAcquisitionHub>();
        hub1.Setup(h => h.Id).Returns(id);
        hub1.Setup(h => h.GroupId).Returns(groupId);

        _manager.AddAcquisitionHub(hub1.Object);

        // Act
        var result = _manager.RemoveHub(hub1.Object);

        // Assert
        Assert.True(result);
        Assert.Equal(0, _manager.TotalGroups);
    }

    #endregion

    #region GetAcquisitionView

    [Fact]
    public void GetAcquisitionView_NewHub_AddsGroup_ReturnsView()
    {
        // Arrange
        var hubId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(hubId);
        hub.Setup(h => h.GroupId).Returns(groupId);

        var acquirer = new Mock<IAcquirer>();

        Assert.Equal(0, _manager.TotalGroups);

        // Act
        var view = _manager.GetAcquisitionView(hub.Object, acquirer.Object);

        // Assert
        Assert.NotNull(view);
        Assert.Equal(1, _manager.TotalGroups);
    }

    #endregion
}
