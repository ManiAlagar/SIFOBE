using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Request
{
    public class PharmacyRequest
    {
     
        public long? Id { get; set; }

        public string PharmacyName { get; set; }


        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsNew { get; set; }
    }
}
