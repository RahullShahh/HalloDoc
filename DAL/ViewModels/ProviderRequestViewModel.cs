using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ProviderRequestViewModel
    {
        public int? RequestId { get; set; }
        public int? CallType { get; set; }
        public string? Email { get; set; }
        public string PatientName { get; set; }
        public int RequestType { get; set; }
        public bool IsHouseCall { get; set; }
        public string? PatientPhone { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsFinalize { get; set; } = false;
    }
}
