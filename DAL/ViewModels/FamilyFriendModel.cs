using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class FamilyFriendModel
    {
        public PatientModel PatientModel { get; set; }
       
        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[A-Za-z\\s]{1,}[\\.]{0,1}[A-Za-z\\s]{0,}$", ErrorMessage = "Enter Valid Name")]
        public string? FirstName { get; set; }
        
        [RegularExpression("^[A-Za-z\\s]{1,}[\\.]{0,1}[A-Za-z\\s]{0,}$", ErrorMessage = "Enter Valid Name")]
        public string? LastName { get; set; }
       
        [Required(ErrorMessage = "Email cannot be kept empty")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string? FriendFamilyPhoneNo { get; set; }
       
        [Required(ErrorMessage ="Kindly specify your relation with patient")]
        public string? Relation {  get; set; }
        public List<Region>? PatientRegions { get; set; }
        public string? FriendFamilyCountryCode {  get; set; }
    }
}
