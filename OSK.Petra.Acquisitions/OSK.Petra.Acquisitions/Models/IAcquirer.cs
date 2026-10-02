using System;

namespace OSK.Petra.Acquisitions.Models;

public interface IAcquirer
{
    int OwnerId { get; }
    
    int TeamId { get; }

    Guid? Id { get; }
}
