namespace SIFO.Model.Request
{
    public class UserRequest
    {
        public long? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? FiscalCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public long? RoleId { get; set; }
        public string? ProfileImg { get; set; }
        public long AuthenticationType { get; set; }
        public bool IsActive { get; set; } = true;
        public long CountryId { get; set; }    
        public List<long>? PharmacyIds { get; set; }
        public List<long>? HospitalIds { get; set; }
    }
}
