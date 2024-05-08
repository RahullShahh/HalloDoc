using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentDataLinkLayer.ViewModels
{
    public class BookDataViewModel
    {
        public List<BookRecordsViewModel> BookRecords { get; set; }
        public string SearchInput {  get; set; }
    }
}
