using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ConciergeModel
    {
        [Required(ErrorMessage = "First name cannot be kept empty")]        
        [RegularExpression("^[A-Za-z\\s]{1,}[\\.]{0,1}[A-Za-z\\s]{0,}$", ErrorMessage = "Enter Valid Name")]
        public string ConciergeFirstName{ get; set; }
       
        [RegularExpression("^[A-Za-z\\s]{1,}[\\.]{0,1}[A-Za-z\\s]{0,}$", ErrorMessage = "Enter Valid Name")]
        public string? ConciergeLastName { get; set;}
        
        [Required(ErrorMessage ="Phone number cannot be kept empty")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string ConciergePhoneNo {  get; set; }
        public string? ConciergeCountryCode {  get; set; }

        [Required(ErrorMessage = "Email cannot be kept empty")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string ConciergeEmail { get; set; } 
        public string? ConciergePropertyName {  get; set; }
        public string? ConciergeStreet {  get; set; }
        public string? ConciergeCity {  get; set; }
        
        [Required(ErrorMessage ="Kindly select a state")]
        public int ConciergeState {  get; set; }
        
        [StringLength(6)]
        public string? ConciergeZipCode { get; set; }
        public string? PatientSymptoms {  get; set; }

        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[A-Za-z\\s]{1,}[\\.]{0,1}[A-Za-z\\s]{0,}$", ErrorMessage = "Enter Valid First Name")]
        public string PatientFirstName {  get; set; }

        [RegularExpression("^[A-Za-z\\s]{1,}[\\.]{0,1}[A-Za-z\\s]{0,}$", ErrorMessage = "Enter Valid Last Name")]
        public string? PatientLastName { get; set;}

        [Required(ErrorMessage = "Date of Birth cannot be empty")]
        //[DateNotInFutureAttribute(ErrorMessage = "Future date cannot be selected")]
        public DateTime? PatientDateOfBirth {  get; set; }

        [Required(ErrorMessage = "Email cannot be kept empty")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string PatientEmail {  get; set; }

        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string PatientPhoneNo { get;set; }
        public string? PatientCountryCode { get;set; }
        public string? PatientRoomNo {  get; set; }
        public List<Region>? Regions { get; set; }

        //public class DateNotInFutureAttribute : ValidationAttribute
        //{
        //    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        //    {
        //        var date = (DateTime?)value;
        //        if (date > DateTime.Now)
        //        {
        //            return new ValidationResult(ErrorMessage);
        //        }
        //        return ValidationResult.Success;
        //    }
        //}
    }
}
