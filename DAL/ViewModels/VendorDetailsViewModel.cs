using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class VendorDetailsViewModel
    {
        public string? UserName { get; set; }

        public List<Healthprofessionaltype>? Healthprofessionaltypes { get; set; }

        public List<VendorDetailsTableViewModel>? VendorsTable { get; set; }
    }
}
