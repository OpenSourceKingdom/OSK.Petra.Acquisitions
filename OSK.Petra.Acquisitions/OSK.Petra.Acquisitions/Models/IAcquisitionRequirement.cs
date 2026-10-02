namespace OSK.Petra.Acquisitions.Models;

public interface IAcquisitionRequirement
{
    bool IsMet(AcquisitionRequirementContext context);
}
