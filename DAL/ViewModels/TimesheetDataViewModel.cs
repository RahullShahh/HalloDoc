using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimesheetDataViewModel
    {
        //public int TimesheetId {  get; set; }
        public int TimesheetDetailId {  get; set; }
        public DateOnly TimesheetDates { get; set; }
        public bool IsHoliday { get; set; } = false;
        public decimal? TotalWorkingHours {  get; set; }
        public int? NoOfHouseCalls {  get; set; }
        public int? NoOfPhoneConsults {  get; set; }
    }
}
