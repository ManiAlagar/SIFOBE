namespace SIFO.Model.Response
{
    public class TokenResponse
    { 
        public string? UserId { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public long? RoleId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Email {  get; set; } = string.Empty;
        public string ParentRoleId {  get; set; } 
    }
}
