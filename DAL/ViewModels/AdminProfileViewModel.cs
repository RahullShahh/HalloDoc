using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminProfileViewModel
    {
        public string username {  get; set; }
        public string password { get; set; }    
        public int statusId {  get; set; }
        public int roleId { get; set; }
        public string firstname { get; set; } 
        public string lastname { get; set; }
        public string email {  get; set; }
        public string confirmEmail {  get; set; }
        public string phoneNo {  get; set; }
        public string address1 {  get; set; }
        public string address2 { get; set; }
        public string city {  get; set; }
        public int regionId{  get; set; }
        public string zipcode { get; set; }
        public string? billingPhone {  get; set; }
        public int adminId {  get; set; }
        public  List<Region> region { get; set; }
        public List<Status> statuses { get; set; }
        public List<Role> roles { get; set; }
        public List<Region> States { get; set; }
        public Adminregion selectedLocations {  get; set; }
        public List<int> chosenLocations { get; set; }
        public Admin? ResidentialRegion {  get; set; }
        
    }
}
