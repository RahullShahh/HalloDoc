using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class SearchRecordsTableViewModel
    {
        public string PatientName { get; set; }
        public string Requestor { get; set; }
        public DateOnly DateOfService { get; set; }
        public DateOnly CloseCaseDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string RequestStatus { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string PatientNotes { get; set; }
        public string PhysicianName {  get; set; }
        public string CancelledByPhysicianNotes { get; set; }
        public int RequestId {  get; set; }
    }
}
