using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimesheetDataViewModel
    {
        public string TimesheetDates { get; set; }
        public bool IsHoliday {  get; set; }
        public double TotalWorkingHours {  get; set; }
        public int NoOfHouseCalls {  get; set; }
        public int NoOfPhoneConsults {  get; set; }
    }
}
