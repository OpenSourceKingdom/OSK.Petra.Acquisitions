namespace OSK.Petra.Acquisitions.Models;

public interface IAcquireableContext
{
    AcquisitionChoiceConfiguration Configuration { get; }

    IAcquireable Acquireable { get; }


}
