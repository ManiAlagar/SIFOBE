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
    }
}
