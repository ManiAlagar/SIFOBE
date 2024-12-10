using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Response
{
    public class ValidationResult
    {
        public bool IsAICExists { get; set; } = false;
        public bool IsRegionDuplicated { get; set; } = false;
        public bool DrugRegionExists { get; set; } = true;
        public bool RegionExists { get; set; } = true;
        public bool DDRegionExists { get; set; } = true;
        public bool DPCRegionExists { get; set; } = true;
    }

}
