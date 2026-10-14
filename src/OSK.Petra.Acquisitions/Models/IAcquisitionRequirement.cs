namespace OSK.Petra.Acquisitions.Models;

/// <summary>
/// A requirement that must be met in order for <see cref="IAcquisition"/> to be available in a view
/// </summary>
public interface IAcquisitionRequirement
{
    /// <summary>
    /// Determines if the given context meets this requirement
    /// </summary>
    /// <param name="context">The context to validate against the requirement</param>
    /// <returns>Whether the context met the requirement</returns>
    bool IsMet(AcquisitionRequirementContext context);
}
