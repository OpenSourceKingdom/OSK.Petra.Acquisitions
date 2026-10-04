namespace OSK.Petra.Acquisitions.Models;

public readonly struct AcquisitionProgress(double progress)
{
    #region Variables

    public double Value { get; } = progress;

    #endregion
}
