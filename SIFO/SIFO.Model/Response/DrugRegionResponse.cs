using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Response
{
    public class DrugRegionResponse
    {
        public long? DrugsRegionsId { get; set; }
        public long RegionId { get; set; }
        public string? RegionName { get; set; }
        public string DrugType { get; set; }
        public long DrugId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
