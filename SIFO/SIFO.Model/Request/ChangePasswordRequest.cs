using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Request
{
    public class ChangePasswordRequest
    {
        //public string Email { get; set; }
        public long Id { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
    }
}
