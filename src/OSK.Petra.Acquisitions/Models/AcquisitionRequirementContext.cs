using OSK.Petra.Acquisitions.Internal.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// A context that provides insights for acquisition requirement validation
/// </summary>
public class AcquisitionRequirementContext
{
    #region Variables

    /// <summary>
    /// The services associated wiht the acquisition process
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// The acquirer who initiated the acquisition process
    /// </summary>
    public IAcquirer Acquirer { get; }

    /// <summary>
    /// The thing being acquired
    /// </summary>
    public IAcquireable Acquireable { get; }

    private readonly OwnerAcquisitionState _state;

    #endregion

    #region Constructors

    internal AcquisitionRequirementContext(IAcquirer acquirer, IAcquireable acquireable, OwnerAcquisitionState ownerState, IServiceProvider serviceProvider)
    {
        Acquirer = acquirer ?? throw new ArgumentNullException(nameof(acquirer));
        Acquireable = acquireable ?? throw new ArgumentNullException(nameof(acquireable));
        _state = ownerState ?? throw new ArgumentNullException(nameof(ownerState));
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    #endregion

    #region Api

    /// <summary>
    /// Attempts to get the progrewssion details for a given acquireable
    /// </summary>
    /// <param name="pathAcquisitionId">The acquisition root path id</param>
    /// <param name="acquireableId">The id for the thing being acquired</param>
    /// <param name="tierProgression">The details if available</param>
    /// <returns>Whether the details exist in the context</returns>
    public bool TryGetChoiceProgression(Guid pathAcquisitionId, Guid acquireableId, [NotNullWhen(true)] out AcquisitionProgressionDetails? tierProgression)
    {
        if (_state.AcquisitionPathStates.TryGetValue(pathAcquisitionId, out var pathState))
        {
            tierProgression = new(acquireableId, pathState.ChoiceStates[acquireableId].TotalAcquisitions);
            return true;
        }

        tierProgression = null;
        return false;
    }

    #endregion
}
