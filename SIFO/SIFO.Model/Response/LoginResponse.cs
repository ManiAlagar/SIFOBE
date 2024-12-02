namespace SIFO.Model.Response
{
    public class LoginResponse
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuResponse>? MenuAccess { get; set; }
    }
}
