using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ProviderLocationViewModel
    {
        public IEnumerable<PhyLocationRow> locationList { get; set; }
        public string ApiKey { get; set; }
    }
}
