using Microsoft.EntityFrameworkCore;

namespace SIFOYarpGateway.Models
{
    public class GatewayDbContext : DbContext
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options) { }

        public DbSet<Routes> Routes { get; set; }
        public DbSet<Clusters> Clusters { get; set; }
    }
}
