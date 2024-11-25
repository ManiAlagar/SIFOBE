using Yarp.ReverseProxy.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace SIFOYarpGateway
{
    public class ServiceCollectionExtension
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProxyConfigProvider, DatabaseProxyConfigProvider>();
            services.AddSingleton<DatabaseProxyConfigProvider>();
            services.AddHostedService<RefreshService>();
            //services.AddReverseProxy();

            var timeoutSetting = configuration.GetValue<TimeSpan>("ReverseProxy:HttpClientDefaults:Timeout");
            services.AddReverseProxy()
                    .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                    .ConfigureHttpClient((context, httpClient) =>
                    {
                        httpClient.ConnectTimeout = timeoutSetting; // Apply global timeout
                    });
        }
    }
}
