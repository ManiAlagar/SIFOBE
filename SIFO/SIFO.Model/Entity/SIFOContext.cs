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
        public virtual DbSet<OtpRequest> OtpRequests { get; set; }
        public virtual DbSet<AuthenticationType> AuthenticationType { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageRoleMapping> PageRoleMapping { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Pharmacy> Pharmacies { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<AddressDetail> AddressDetails { get; set; }
        public virtual DbSet<PharmacyType> PharmacyTypes { get; set; }
        public virtual DbSet<Calendar> Calendar{ get; set; }
        public virtual DbSet<UserSessionManagement> UserSessionManagements{ get; set; }
    }
}
