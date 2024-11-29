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
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
    }
}
