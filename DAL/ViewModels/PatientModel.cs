using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static DAL.ViewModels.ConciergeModel;
namespace DAL.ViewModels
{
    public class PatientModel
    {
        public string? Symptoms { get; set; }
        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]        
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name cannot be kept empty")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid last name.")]
        public string? LastName { get; set; }
        
        [Required(ErrorMessage ="Date of Birth cannot be empty")]
       // [DateNotInFutureAttribute(ErrorMessage = "Birthdate cannot be in future")]
        public DateTime? DateOfBirth { get; set; }
        
        [Required(ErrorMessage ="Email cannot be kept empty")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string Email { get; set; } 
        
        [Required(ErrorMessage ="Phone number cannot be kept empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNo { get; set; } = "";
       
        public string? Street { get; set; }
        
        public string? City { get; set; }

        [Required(ErrorMessage = "Kindly select a state")]
        public int State { get; set; }
        
        [StringLength(6)]
        public string? ZipCode { get; set; }
        
        public string CountryCode { get; set; } = string.Empty;
        
        public string? RoomSuite { get; set; }
        
        public IFormFile? File { get; set; }

        [RegularExpression("(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must contain 1 capital, 1 small, 1 Special symbol and at least 8 characters")]
        public string? Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
        public string? ConfirmPass { get; set; }
        public List<Region>? Regions { get; set; }
    }
}
