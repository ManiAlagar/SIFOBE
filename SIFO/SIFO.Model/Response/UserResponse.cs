using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Response
{
    public class UserResponse
    {
        public long? UserId { get; set; }
        public long? TenantId { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
    }
}
