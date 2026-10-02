using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Requirements.Dependency;

public class AcquisitionDependencyRequirement(IEnumerable<AcquisitionDependency> dependencies) : IAcquisitionRequirement
{
    #region IAcquisitionRequirement

    public bool IsMet(AcquisitionRequirementContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return dependencies.All(dependency => context.TryGetChoice(dependency.AcquireableId, out var choiceState) && choiceState.TotalAcquisitions >= dependency.RequiredAcquisitionCount);
    }

    #endregion
}
