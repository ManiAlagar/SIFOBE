using SIFO.Model.AutoMapper;
using System.Diagnostics.CodeAnalysis;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.APIService.Master.Service.Implementations;
using SIFO.APIService.Master.Repository.Implementations;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using System.Reflection;
using FluentValidation;
using SendGrid;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Repository.Implementations;
using SIFO.Core.Service.Contracts;
using SIFO.Core.Service.Implementations;

namespace SIFO.APIService.Master
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //Mapper
            services.AddAutoMapper(typeof(MapperProfile).Assembly);
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            //Repositories
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IStateRepository, StateRepository>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddTransient<ITwilioService, TwilioService>();
            services.AddTransient<ITwilioRepository, TwilioRepository>();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            //Services
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IMasterService, MasterService>();
            services.AddTransient<IMasterRepository, MasterRepository>();
            services.AddTransient<ICommonService, CommonService>();

            services.AddHttpContextAccessor();
            return services;
        }
    }
}
