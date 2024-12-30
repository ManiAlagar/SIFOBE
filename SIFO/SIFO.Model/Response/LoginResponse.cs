using SIFO.Model.Entity;

namespace SIFO.Model.Response
{
    public class LoginResponse
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PageResponse>? MenuAccess { get; set; }
        public bool IsFirstAccess { get; set; }
        public bool IsTempPassword { get; set; }
        public List<RoleResponse?> hasCreatePermission { get;set;}
        public List<long> ParentRoleId { get;set;}
    }
}
