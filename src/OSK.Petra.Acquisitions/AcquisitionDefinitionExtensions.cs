using OSK.Petra.Acquisitions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Acquisitions;

public static class AcquisitionDefinitionExtensions
{
    public static IEnumerable<IAcquisitionDefinition> Flatten(this IAcquisitionDefinition definition)
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
    /*
    extension(IAcquisitionDefinition choice)
    {
        public IEnumerable<IAcquisitionDefinition> Flatten()
            => Flatten(choice, []);

        private IEnumerable<IAcquisitionDefinition> Flatten(HashSet<Guid> visited)
        {
            if (visited.Contains(choice.Acquireable.Id))
            {
                yield break;
            }

            visited.Add(choice.Acquireable.Id);
            
            yield return choice;
            
            foreach (var nextChoice in choice.NextChoices)
            {
                foreach (var nestedChoice in nextChoice.Flatten(visited))
                {
                    yield return nestedChoice;
                }
            }
        }
    }
    */
}
