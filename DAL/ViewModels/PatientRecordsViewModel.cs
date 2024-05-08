using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PatientRecordsViewModel
    {
        public string ClientName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ConfirmationNo { get; set; }
        public string ProviderName { get; set; }
        public DateTime? ConcludedDate { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public int Requestid {  get; set; }
        
    }
}
