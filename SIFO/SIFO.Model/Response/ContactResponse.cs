namespace SIFO.Model.Response
{
    public class ContactResponse
    {
        public long Id { get; set; }
        public long HospitalId { get; set; }
        public string ContactName { get; set; }
        public string ContactSurname { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
