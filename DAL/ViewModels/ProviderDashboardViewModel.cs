using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ProviderDashboardViewModel
    {
        public List<ProviderRequestViewModel> RequestList { get; set; }
        public IEnumerable<Casetag> casetags { get; set; }
        public IEnumerable<Region> regions { get; set; }
        public IEnumerable<Physician> physicians { get; set; }
        public DashboardFilter filterOptions { get; set; }
        public string UserName { get; set; }
        public int DashboardStatus { get; set; }
        public int NewReqCount { get; set; }
        public int PendingReqCount { get; set; }
        public int ActiveReqCount { get; set; }
        public int ConcludeReqCount { get; set; }
        public int CurrentPage { get; set; }
    }
}

