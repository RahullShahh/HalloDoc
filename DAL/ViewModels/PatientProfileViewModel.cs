using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PatientProfileViewModel
    {
        public int UserId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public DateOnly Date {  get; set; }
        public string PhoneNo {  get; set; }
        public string city {  get; set; }
        public string street {  get; set; }
        public string country { get; set; }
        public string zipcode {  get; set; }
        public string state {  get; set; }
        public string email {  get; set; }
        public int userid { get; set; }

    }
}
