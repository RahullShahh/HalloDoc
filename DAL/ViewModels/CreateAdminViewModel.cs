using DAL.DataModels;
using System.ComponentModel.DataAnnotations;
namespace DAL.ViewModels
{
    public class CreateAdminViewModel
    {
        public List<Region> Regions { get; set; }
        [Required(ErrorMessage ="Enter Username")]
        public string? UserName { get; set; }
        [Required(ErrorMessage ="Select a state from dropdown.")]
        public int? state { get; set; }
        public List<Role>? roles { get; set; }
        public string? Role { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 8 characters long.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid last name.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Confirm Email is required")]
        [DataType(DataType.EmailAddress)]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string? ConfirmEmail { get; set; }
        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? PhoneNumAspNetUsers { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        [StringLength(6)]
        public string? zip { get; set; }
        public string? MobileNumAdmin { get; set; }
        public int? SelectedStateId { get; set; }
        public List<int>? SelectedRegions { get; set; }
        public List<CheckboxList_model>? statesForChecked { get; set; }
    }
    public class CheckboxList_model
    {
        public int? Value { get; set; }
        public Boolean? Selected { get; set; }
    }
}
