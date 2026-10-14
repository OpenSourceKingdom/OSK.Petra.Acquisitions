using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Acquisitions.Requirements.Dependency;

/// <summary>
/// A requirement that uses dependencies to determine if it is met
/// </summary>
/// <param name="dependencies">The collection of dependencies to validate</param>
public class AcquisitionDependencyRequirement(IEnumerable<AcquisitionDependency> dependencies) : IAcquisitionRequirement
{
    #region IAcquisitionRequirement

    public bool IsMet(AcquisitionRequirementContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var met = dependencies
                    // Dependency can not require itself
                    .Where(dependency => dependency.AcquireableId != context.Acquireable.Id)
                    .All(dependency => context.TryGetChoiceProgression(dependency.AcquisitionPathId, dependency.AcquireableId, out var choiceTierProgression) && choiceTierProgression.Value.TotalAcquisitions >= dependency.RequiredAcquisitionCount);
        return met;
    }

    #endregion
}
