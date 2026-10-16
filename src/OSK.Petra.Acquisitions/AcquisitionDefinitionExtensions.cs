using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions;

public static class AcquisitionDefinitionExtensions
{
    /// <summary>
    /// Expands the choice definition to get all the choices within the definition tree
    /// </summary>
    /// <param name="definition">The definition to expand</param>
    /// <returns>The collection of all choices on the definition</returns>
    public static IEnumerable<IAcquisitionDefinition> ExpandChoices(this IAcquisitionDefinition definition)
        => Flatten(definition, []);

    private static IEnumerable<IAcquisitionDefinition> Flatten(IAcquisitionDefinition choice, HashSet<Guid> visited)
    {
        if (visited.Contains(choice.Acquireable.Id))
        {
            yield break;
        }

        visited.Add(choice.Acquireable.Id);

        yield return choice;

        foreach (var nextChoice in choice.NextChoices)
        {
            foreach (var nestedChoice in Flatten(nextChoice, visited))
            {
                yield return nestedChoice;
            }
        }
    }
}
