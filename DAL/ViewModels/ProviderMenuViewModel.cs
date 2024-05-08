using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ProviderMenuViewModel
    {
        public List<Region> Region {  get; set; }
        public List<Providers> providers { get; set; }
        
    }
}
