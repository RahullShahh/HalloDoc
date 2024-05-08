using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminRequestsViewModel
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Requestor { get; set; }
        public DateTime Requesteddate { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string OtherPhoneNo{ get; set; }
        public int requestType {  get; set; }
        public int status { get; set; }
        public string chatWith { get; set; }
        public string physicianName { get; set; }
        public DateOnly servicedate { get; set; }
        public string region { get; set; }
        public int requestid {  get; set; }
        public string email {  get; set; }
        public bool isFinalize {  get; set; }
        public int? PhysicianId {  get; set; }
        public int? calltype {  get; set; }
    }
}
