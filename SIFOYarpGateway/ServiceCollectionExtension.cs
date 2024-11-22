using Yarp.ReverseProxy.Configuration;

namespace SIFOYarpGateway
{
    public class ServiceCollectionExtension
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IProxyConfigProvider, DatabaseProxyConfigProvider>();
            services.AddSingleton<DatabaseProxyConfigProvider>();
            services.AddHostedService<RefreshService>();
            services.AddReverseProxy();
        }
    }
}
