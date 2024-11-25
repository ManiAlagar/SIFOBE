namespace SIFO.Model.Response
{
    public class LoginResponse
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public long[]? Roles { get; set; }
        public string[]? RoleNames { get; set; }
        public IEnumerable<MenuResponse>? MenuAccess { get; set; }
    }
}
