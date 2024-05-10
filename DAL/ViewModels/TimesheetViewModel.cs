using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimesheetViewModel
    {
        public int TimesheetId {  get; set; }
        public List<TimesheetDataViewModel> TimesheetData { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set;}

    }
}
