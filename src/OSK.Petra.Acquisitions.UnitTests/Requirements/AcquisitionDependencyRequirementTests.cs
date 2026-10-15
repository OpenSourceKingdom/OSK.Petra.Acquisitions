using Moq;
using OSK.Petra.Acquisitions.Internal.Models;
using OSK.Petra.Acquisitions.Models;
using OSK.Petra.Acquisitions.Requirements.Dependency;

namespace OSK.Petra.Acquisitions.UnitTests.Requirements;

public class AcquisitionDependencyRequirementTests
{
    #region Variables

    private readonly List<AcquisitionDependency> _dependencies = [];

    private readonly AcquisitionDependencyRequirement _requirement;

    #endregion

    #region Constructors

    public AcquisitionDependencyRequirementTests()
    {
        _requirement = new(_dependencies);
    }

    #endregion

    #region  IsMet

    [Fact]
    public void IsMet_NullContext_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _requirement.IsMet(null!));
    }

    [Fact]
    public void IsMet_NoDependencies_ReturnsTrue()
    {
        // Arrange
        var ownerState = new OwnerAcquisitionState()
        {
            OwnerId = 1
        };
        var context = new AcquisitionRequirementContext(Mock.Of<IAcquirer>(), Mock.Of<IAcquireable>(), ownerState, Mock.Of<IServiceProvider>());
        
        // Act/Assert
        Assert.True(_requirement.IsMet(context));
    }

    [Fact]
    public void IsMet_HasDependency_NoAcquisitions_NotSatisfied_ReturnsFalse()
    {
        // Arrange
        var ownerState = new OwnerAcquisitionState()
        {
            OwnerId = 1
        };
        _dependencies.Add(new(Guid.NewGuid(), 1));
        var context = new AcquisitionRequirementContext(Mock.Of<IAcquirer>(), Mock.Of<IAcquireable>(), ownerState, Mock.Of<IServiceProvider>());

        // Act/Assert
        Assert.False(_requirement.IsMet(context));
    }


    [Fact]
    public void IsMet_HasDependency_SomeAcquisitions_NotSatisfied_ReturnsFalse()
    {
        // Arrange
        var ownerState = new OwnerAcquisitionState()
        {
            OwnerId = 1
        };
        var acquireableId = Guid.NewGuid();

        ownerState.AcquisitionPathStates.Add(acquireableId, new()
        {
            RootChoiceId = acquireableId,
            ChoiceStates = new()
            {
                {
                    acquireableId,
                    new()
                    {
                        AcquireableId = acquireableId,
                        TotalAcquisitions = 1
                    }
                }
            }
        });

        _dependencies.Add(new(acquireableId, 2));
        var context = new AcquisitionRequirementContext(Mock.Of<IAcquirer>(), Mock.Of<IAcquireable>(), ownerState, Mock.Of<IServiceProvider>());

        // Act/Assert
        Assert.False(_requirement.IsMet(context));
    }

    [Fact]
    public void IsMet_HasDependency_Satisfied_ReturnsTrue()
    {
        // Arrange
        var ownerState = new OwnerAcquisitionState()
        {
            OwnerId = 1
        };
        var acquireableId = Guid.NewGuid();

        ownerState.AcquisitionPathStates.Add(acquireableId, new()
        {
            RootChoiceId = acquireableId,
            ChoiceStates = new()
            {
                {
                    acquireableId,
                    new()
                    {
                        AcquireableId = acquireableId,
                        TotalAcquisitions = 1
                    }
                }
            }
        });

        _dependencies.Add(new(acquireableId, 1));
        var context = new AcquisitionRequirementContext(Mock.Of<IAcquirer>(), Mock.Of<IAcquireable>(), ownerState, Mock.Of<IServiceProvider>());

        // Act/Assert
        Assert.True(_requirement.IsMet(context));
    }

    #endregion
}
