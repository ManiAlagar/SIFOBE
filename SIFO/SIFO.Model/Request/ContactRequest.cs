namespace SIFO.Model.Request
{
    public class ContactRequest
    {
        public long? Id { get; set; }
        public string ContactName { get; set; }
        public string ContactSurname { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsActive { get; set; } = true;
        //public bool? IsDeleted { get; set; }
        //public bool? IsNew { get; set; }
    }
}
