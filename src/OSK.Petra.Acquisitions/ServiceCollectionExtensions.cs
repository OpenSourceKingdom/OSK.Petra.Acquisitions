using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Petra.Acquisitions.Internal.Services;
using OSK.Petra.Acquisitions.Ports;

namespace OSK.Petra.Acquisitions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the acquisition system and related services to the dependency container
        /// </summary>
        /// <returns>The services for chaining</returns>
        public IServiceCollection AddAcquisitions()
        {
            services.TryAddSingleton<IAcquisitionSystem, AcquisitionSystem>();

            return services;
        }
    }
}
