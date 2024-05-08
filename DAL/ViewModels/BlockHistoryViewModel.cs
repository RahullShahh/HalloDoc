using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class BlockHistoryViewModel
    {
        public string PatientName {  get; set; }
        public string PhoneNo {  get; set; }
        public string Email {  get; set; }
        public DateTime CreatedDate { get; set; }
        public string Notes {  get; set; }
        public bool IsActive {  get; set; }
        public int RequestId {  get; set; }
    }
}
