using System.Diagnostics.CodeAnalysis;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.APIService.Master.Repository.Implementations;
using FluentValidation.AspNetCore;
using FluentValidation;
using SIFO.Model.Validator;
using System.Text.Json;
using SIFO.Model.Request;
using SIFO.APIService.Master.Service.Implementations;
using SIFO.Model.AutoMapper;

namespace SIFO.APIService.Master
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            //Validators
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            }).AddFluentValidation();

            services.AddSingleton<IValidator<CountryRequest>, CountryValidator>();
            services.AddSingleton<IValidator<StateRequest>, StateValidator>();
            services.AddSingleton<IValidator<CityRequest>, CityValidator>();

            //Mapper
            services.AddAutoMapper(typeof(MapperProfile).Assembly);

            //Repositories
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IStateRepository, StateRepository>();

            //Services
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICityService, CityService>();

            return services;
        }
    }
}
