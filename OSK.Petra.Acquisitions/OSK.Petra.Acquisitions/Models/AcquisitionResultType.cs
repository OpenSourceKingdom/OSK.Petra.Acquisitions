namespace OSK.Petra.Acquisitions.Models;

public enum AcquisitionOutcome
{
    Success,
    Delayed,

    InsufficientProvisions,
    InvalidAcquireable,
    MaxQueueSizeReached,
    MaxAcquisitionsReached
}
