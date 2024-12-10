using SendGrid;
using SIFO.Model.Entity;
using SIFO.Core.Service.Contracts;
using SIFO.Core.Repository.Implementations;
using SIFO.Core.Service.Implementations;
using SIFO.Core.Repository.Contracts;

namespace SIFO.APIService.ThirdPartyAlert
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            services.AddTransient<SIFOContext>();
            services.AddTransient<ITwilioService, TwilioService>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddTransient<ITwilioRepository, TwilioRepository>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddMemoryCache();
            return services;
        }
    }
}
