namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// Represents the progress for a given acquisition
/// </summary>
/// <param name="progress">The acuqiistion progress, from 0-1</param>
public readonly struct AcquisitionProgress(double progress)
{
    #region Variables

    /// <summary>
    /// The acuqiistion progress, from 0-1
    /// </summary>
    public double Value { get; } = progress;

    #endregion
}
