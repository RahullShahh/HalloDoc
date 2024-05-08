using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class SearchRecordViewModel
    {
        public List<Requeststatus> RequestStatus { get; set; }
        public List<Requesttype> RequestType { get; set; }
        public List<SearchRecordsTableViewModel> SearchRecordsTableData { get; set; }
    }
}
