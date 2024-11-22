using SIFOYarpGateway.Models;
using Yarp.ReverseProxy.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SIFOYarpGateway
{
    public class DatabaseProxyConfigProvider : IProxyConfigProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private volatile CustomProxyConfig _config;
        private readonly object _sync = new();
        private readonly IConfiguration _configuration;

        public DatabaseProxyConfigProvider(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            RefreshConfig();
        }

        public IProxyConfig GetConfig() => _config;

        public void RefreshConfig()
        {
            lock (_sync)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();

                var clusterEntities = dbContext.Clusters.ToList();
                var routeEntities = dbContext.Routes.ToList();

                //var clusters = dbContext.Clusters
                //    .Select(cluster => new ClusterConfig
                //    {
                //        ClusterId = cluster.ClusterId,
                //        Destinations = new Dictionary<string, DestinationConfig>
                //        {
                //            { "default", new DestinationConfig { Address = cluster.Destinations } }
                //        }
                //    })
                //    .ToList();

                //var routes = routeEntities
                //    .Select(route => new RouteConfig
                //    {
                //        RouteId = route.RouteId,
                //        ClusterId = route.ClusterId,
                //        Match = new RouteMatch { Path = route.PathPattern }
                //    })
                //    .ToList();

                // Load routes from database
                var routes = dbContext.Routes.Select(rc => new RouteConfig
                {
                    RouteId = rc.RouteId,
                    ClusterId = rc.ClusterId,
                    Match = new RouteMatch { Path = rc.PathPattern }
                }).ToList();

                if (!routes.Any())
                {
                    routes.Add(new RouteConfig
                    {
                        RouteId = "default-route",
                        ClusterId = "default-cluster",
                        Match = new RouteMatch { Path = "*" }
                    });
                }

                // Load clusters from database
                var clusters = dbContext.Clusters
                    .ToList()
                    .GroupBy(cc => cc.ClusterId)
                    .Select(g => new ClusterConfig
                    {
                        ClusterId = g.Key,
                        Destinations = g
                            .ToDictionary(
                                keySelector: cc => cc.Destinations ?? _configuration["ProxyConfig:DefaultUrl"],  // Fallback to default URL
                                elementSelector: cc => new DestinationConfig { Address = cc.Destinations ?? _configuration["ProxyConfig:DefaultUrl"]
                                }),
                        LoadBalancingPolicy = "RoundRobin"
                    })
                    .ToList();

                _config = new CustomProxyConfig(routes, clusters);
            }
        }
    }

    public class CustomProxyConfig : IProxyConfig
    {
        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }

        public CustomProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(new CancellationToken(false));
        }
    }
}
