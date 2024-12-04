using SIFO.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Request
{
    public class HospitalRequest
    {

        public string HospitalName { get; set; }
        public long Region { get; set; }
        public long? AddressId { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
        public string ASL { get; set; }
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public string CAB { get; set; }

        public List<ContactRequest> Contact { get; set; }
        public List<PharmacyRequest> Pharmacy { get; set; }

   
    }

}
