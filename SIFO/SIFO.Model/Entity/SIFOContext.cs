using Microsoft.EntityFrameworkCore;

namespace SIFO.Model.Entity
{
    public class SIFOContext:DbContext
    {
        public SIFOContext()
        {
        }
        public SIFOContext(DbContextOptions<SIFOContext> options) : base(options)
        {
        } 
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<OtpRequest> OtpRequests { get; set; }
        public virtual DbSet<AuthenticationType> AuthenticationType { get; set; }
    }
}
