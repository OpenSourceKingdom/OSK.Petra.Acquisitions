using Moq;
using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Ports;
using OSK.Petra.Provisions.Models;

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

    #region InitiateAcquisition
    
    [Fact]
    public void InitiateAcquisition_NullAcquirer_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var path = Mock.Of<IAcquireable>();
        var selected = Mock.Of<IAcquireable>();
        var provisions = new List<Provision>();

        Assert.Throws<ArgumentNullException>(() => _group.InitiateAcquisition(hub, null!, path, selected, provisions));
    }

    [Fact]
    public void InitiateAcquisition_NullPathAcquireable_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var acquirer = Mock.Of<IAcquirer>();
        var selected = Mock.Of<IAcquireable>();
        var provisions = new List<Provision>();

        Assert.Throws<ArgumentNullException>(() => _group.InitiateAcquisition(hub, acquirer, null!, selected, provisions));
    }

    [Fact]
    public void InitiateAcquisition_NullSelectedAcquireable_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var acquirer = Mock.Of<IAcquirer>();
        var path = Mock.Of<IAcquireable>();
        var provisions = new List<Provision>();

        Assert.Throws<ArgumentNullException>(() => _group.InitiateAcquisition(hub, acquirer, path, null!, provisions));
    }

    [Fact]
    public void InitiateAcquisition_NullProvisions_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var acquirer = Mock.Of<IAcquirer>();
        var path = Mock.Of<IAcquireable>();
        var selected = Mock.Of<IAcquireable>();

        Assert.Throws<ArgumentNullException>(() => _group.InitiateAcquisition(hub, acquirer, path, selected, null!));
    }

    [Fact]
    public void InitiateAcquisition_MaxAcquisitionsReached_ReturnsMaxAcquisitionsReached()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration { MaxAcquisitionCount = 3 });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        choiceState.TotalAcquisitions = 3;

        _group.AddHub(mockHub.Object);

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, []);

        // Assert
        Assert.False(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.MaxAcquisitionsReached, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_MaxConcurrentHubLimitReached_NoQueue_ReturnsMaxQueueSizeReached()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { MaxGroupPendingAcquisitions = 0 });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration()
            {
                MaxConcurrentAcquisitions = 1
            });

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);
        hubQueueState.AcquisitionsInProgress.Add(new()
        {
            Acquirer = Mock.Of<IAcquirer>(),
            Hold = Mock.Of<IAcquisitionHold>()
        });

        _group.AddHub(mockHub.Object, new AcquisitionHubState() 
        { 
            HubId = mockHub.Object.Id, 
            ChoiceStates = new()
            {
                { 
                    mockAcquireable.Object.Id, 
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, []);

        // Assert
        Assert.False(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.MaxQueueSizeReached, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_MaxConcurrentHubGroupLimitReached_NoQueue_ReturnsMaxQueueSizeReached()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { MaxGroupPendingAcquisitions = 0, MaxGroupConcurrentAcquisitions = 1 });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration()
            {
                MaxConcurrentAcquisitions = 5
            });

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);
        hubQueueState.AcquisitionsInProgress.Add(new()
        {
            Acquirer = Mock.Of<IAcquirer>(),
            Hold = Mock.Of<IAcquisitionHold>()
        });

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, []);

        // Assert
        Assert.False(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.MaxQueueSizeReached, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_MaxConcurrentHubLimitReached_MaxHubQueueSizeReached_ReturnsMaxQueueSizeReached()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration());

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration()
            {
                MaxPendingAcquisitions = 1,
                MaxConcurrentAcquisitions = 1
            });

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);
        hubQueueState.PendingAcquisitions.AddFirst(new AcquisitionHoldState() { Acquirer = Mock.Of<IAcquirer>(), Hold = Mock.Of<IAcquisitionHold>() });
        hubQueueState.AcquisitionsInProgress.Add(new()
        {
            Acquirer = Mock.Of<IAcquirer>(),
            Hold = Mock.Of<IAcquisitionHold>()
        });

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, []);

        // Assert
        Assert.False(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.MaxQueueSizeReached, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_MaxConcurrentHubLimitReached_MaxHubGroupQueueSizeReached_ReturnsMaxQueueSizeReached()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { MaxGroupPendingAcquisitions = 1 });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration()
            {
                MaxPendingAcquisitions = 5,
                MaxConcurrentAcquisitions = 1
            });

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);
        hubQueueState.PendingAcquisitions.AddFirst(new AcquisitionHoldState() { Acquirer = Mock.Of<IAcquirer>(), Hold = Mock.Of<IAcquisitionHold>() });
        hubQueueState.AcquisitionsInProgress.Add(new()
        {
            Acquirer = Mock.Of<IAcquirer>(),
            Hold = Mock.Of<IAcquisitionHold>()
        });

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, []);

        // Assert
        Assert.False(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.MaxQueueSizeReached, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_InsufficientProvisions_ReturnsInsufficientProvisions()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var costProvider = new Mock<IAcquisitionCostProvider>();
        costProvider.Setup(m => m.GetCost(It.IsAny<HubAcquisitionContext>()))
            .Returns([new Provision(Guid.NewGuid(), 500)]);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { AcquisitionCostProvider = costProvider.Object });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration());

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, []);

        // Assert
        Assert.False(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.InsufficientProvisions, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_InsufficientProvisions_AllowsInsufficient_NoHold_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var provision = new Provision(Guid.NewGuid(), 500);
        var costProvider = new Mock<IAcquisitionCostProvider>();
        costProvider.Setup(m => m.GetCost(It.IsAny<HubAcquisitionContext>()))
            .Returns([provision]);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { AcquisitionCostProvider = costProvider.Object });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration());

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, [provision], new() { AllowInsufficientProvisions = true });

        // Assert
        Assert.True(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.Success, acquisitionResult.Outcome);
        Assert.NotNull(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_SufficientProvisions_NoHold_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var provision = new Provision(Guid.NewGuid(), 500);
        var costProvider = new Mock<IAcquisitionCostProvider>();
        costProvider.Setup(m => m.GetCost(It.IsAny<HubAcquisitionContext>()))
            .Returns([provision]);

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { AcquisitionCostProvider = costProvider.Object });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration());

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, [provision]);

        // Assert
        Assert.True(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.Success, acquisitionResult.Outcome);
        Assert.NotNull(acquisitionResult.Acquireable);
    }

    [Fact]
    public void InitiateAcquisition_SufficientProvisions_HasHold_ReturnsDelayed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockAcquireable = new Mock<IAcquireable>();
        mockAcquireable.Setup(a => a.Id)
            .Returns(id);

        var provision = new Provision(Guid.NewGuid(), 500);
        var costProvider = new Mock<IAcquisitionCostProvider>();
        costProvider.Setup(m => m.GetCost(It.IsAny<HubAcquisitionContext>()))
            .Returns([provision]);

        var holdProvider = new Mock<IAcquisitionHoldProvider>();
        holdProvider.Setup(m => m.GetHold(It.IsAny<HubAcquisitionContext>()))
            .Returns(Mock.Of<IAcquisitionHold>());

        var mockDefinition = new Mock<IAcquisitionDefinition>();
        mockDefinition.Setup(d => d.Acquireable).Returns(mockAcquireable.Object);
        mockDefinition.Setup(d => d.NextChoices).Returns(new List<IAcquisitionDefinition>());
        mockDefinition.Setup(d => d.Configuration).Returns(new AcquisitionChoiceConfiguration() { AcquisitionCostProvider = costProvider.Object, AcquisitionHoldProvider = holdProvider.Object });

        var mockHub = new Mock<IAcquisitionHub>();
        var hubId = Guid.NewGuid();
        mockHub.Setup(h => h.Id).Returns(hubId);
        mockHub.Setup(h => h.GetAcquisitions())
            .Returns([mockDefinition.Object]);
        mockHub.Setup(h => h.Configuration)
            .Returns(new AcquisitionHubConfiguration());

        var pathState = _ownerState.GetOrAddPathState(id);
        var choiceState = pathState.GetOrCreateState(id);

        var hubQueueState = choiceState.GetOrAddHubQueue(mockHub.Object.Id);

        _group.AddHub(mockHub.Object, new AcquisitionHubState()
        {
            HubId = mockHub.Object.Id,
            ChoiceStates = new()
            {
                {
                    mockAcquireable.Object.Id,
                    new AcquisitionHubChoiceState()
                    {
                        PathId = mockAcquireable.Object.Id,
                        AcquireableId = mockAcquireable.Object.Id,
                        QueueState = hubQueueState
                    }
                }
            }
        });

        // Act
        var acquisitionResult = _group.InitiateAcquisition(mockHub.Object, Mock.Of<IAcquirer>(), mockAcquireable.Object, mockAcquireable.Object, [provision]);

        // Assert
        Assert.True(acquisitionResult.Success);
        Assert.Equal(AcquisitionOutcome.Delayed, acquisitionResult.Outcome);
        Assert.Null(acquisitionResult.Acquireable);
    }

    #endregion

    #region CancelAcquisition

    [Fact]
    public void CancelAcquisition_NullAcquirer_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var path = Mock.Of<IAcquireable>();
        var selected = Mock.Of<IAcquireable>();

        Assert.Throws<ArgumentNullException>(() => _group.CancelAcquisition(hub, null!, path, selected));
    }

    [Fact]
    public void CancelAcquisition_NullPathAcquireable_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var acquirer = Mock.Of<IAcquirer>();
        var selected = Mock.Of<IAcquireable>();

        Assert.Throws<ArgumentNullException>(() => _group.CancelAcquisition(hub, acquirer, null!, selected));
    }

    [Fact]
    public void CancelAcquisition_NullCanceledAcquireable_ThrowsArgumentNullException()
    {
        var hub = Mock.Of<IAcquisitionHub>();
        var acquirer = Mock.Of<IAcquirer>();
        var path = Mock.Of<IAcquireable>();

        Assert.Throws<ArgumentNullException>(() => _group.CancelAcquisition(hub, acquirer, path, null!));
    }

    #endregion

    #region GetAcquisitionView

    [Fact]
    public void GetAcquisitionView_AddsHub_ReturnsView()
    {
        // Arrange
        var hub = new Mock<IAcquisitionHub>();
        hub.Setup(h => h.Id).Returns(Guid.NewGuid());
        hub.Setup(h => h.GetAcquisitions()).Returns(Enumerable.Empty<IAcquisitionDefinition>());
        var acquirer = Mock.Of<IAcquirer>();

        // Act
        var view = _group.GetAcquisitionView(hub.Object, acquirer);

        // Assert
        Assert.NotNull(view);
        Assert.Equal(hub.Object, view.Hub);
        Assert.Equal(acquirer, view.Acquirer);
    }

    #endregion
}