using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public  class ReviewAgreementViewModel
    {
        public string PatientName {  get; set; }
        public string CancelReason {  get; set; }
        public int reqID {  get; set; }
        
    }
}
