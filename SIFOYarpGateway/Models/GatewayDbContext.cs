using Microsoft.EntityFrameworkCore;

namespace SIFOYarpGateway.Models
{
    public class GatewayDbContext : DbContext
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options) { }

        public DbSet<Route> Routes { get; set; }
        public DbSet<Cluster> Clusters { get; set; }
    }
}
