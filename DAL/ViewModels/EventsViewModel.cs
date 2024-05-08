using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class EventsViewModel
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Status { get; set; }
        public int ShiftDetailsId { get; set; }
        public int? Regionid { get; set; }
        public string? Region { get; set; }
    }
}
