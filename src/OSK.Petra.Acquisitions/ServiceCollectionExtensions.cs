using Microsoft.Extensions.DependencyInjection;

namespace OSK.Petra.Acquisitions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAcquisitions()
        {

            return services;
        }
    }
}
