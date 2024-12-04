using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Repository.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Service.Implementations;
using System.Diagnostics.CodeAnalysis;

namespace SIFO.APIService.Hospital
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            services.AddTransient<IHospitalRepository, HospitalRepository>();
            services.AddTransient<IHospitalService, HospitalService>();

            return services;
        }
    }
}
