using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Ports;

namespace OSK.Petra.Acquisitions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAcquisitions()
        {
            services.TryAddSingleton<IAcquisitionSystem, AcquisitionSystem>();

            return services;
        }
    }
}
