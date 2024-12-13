﻿namespace SIFO.Model.Response
{

    public class UserResponse
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ZipCode { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public string ProfileImg { get; set; }
        public string? FiscalCode { get; set; }
        public bool? IsActive { get; set; } = true;
        public long? AuthenticationType { get; set; }
        public string AuthenticationName { get; set; }
        public long? ParentRoleId { get; set; }
    }

}
