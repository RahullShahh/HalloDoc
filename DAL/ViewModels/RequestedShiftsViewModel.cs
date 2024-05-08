using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class RequestedShiftsViewModel
    {
        public int ShiftDetailId { get; set; }
        public string Staff { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string? Region { get; set; }
        public int Regionid { get; set; }
    }
}
