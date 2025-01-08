namespace SIFO.Model.Response
{
    public class PatientsLoginResponse
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public long? AuthenticationType { get; set; }
        public DateTime? PswdUpdatedAt { get; set; }
    }
}
