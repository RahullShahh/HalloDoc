using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ViewCaseViewModel
    {
        public string patientnotes {  get; set; }
        public string patientfirstname {  get; set; }
        public string patientlastname { get;set; }
        public string dob {  get; set; }
        public string countrycode {  get; set; }
        public string region {  get; set; }
        public string address {  get; set; }
        public string rooms { get; set; }
        public string patientphone {  get; set; }
        public string patientemail {  get; set; }
        public string confirmationNo {  get; set; }
        public string businessNameOrAddress { get; set; }
        public int requestID {  get; set; }
    }
}