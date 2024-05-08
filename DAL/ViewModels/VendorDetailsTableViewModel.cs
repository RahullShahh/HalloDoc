using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class VendorDetailsTableViewModel
    {
        public string? profession { get; set; }

        public string? businessName { get; set; }

        public string? email { get; set; }

        public string? faxNumber { get; set; }

        public string? phone { get; set; }

        public string? businessContact { get; set; }

        public int? vendorId { get; set; }
    }
}
