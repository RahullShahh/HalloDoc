using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class DashboardFilter
    {
        public string PatientSearchText { get; set; }
        public int RegionFilter { get; set; }
        public int RequestTypeFilter { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int status { get; set; }
        public int page {  get; set; }
    }
}
