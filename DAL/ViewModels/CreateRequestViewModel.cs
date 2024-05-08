using DAL.DataModels;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class CreateRequestViewModel
    {
        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public string FirstName {  get; set; }
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid last name.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNo { get; set; } = "";
        [Required(ErrorMessage = "Email cannot be kept empty")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string Email {  get; set; }
        [Required(ErrorMessage = "Date of Birth cannot be empty")]
        public DateOnly BirthDate { get; set; }
        public string street { get; set; }
        public string City { get; set; }
        [Required(ErrorMessage = "Kindly select a state")]
        public string State { get; set; }
        [StringLength(6)]
        public string? Zipcode { get; set; }
        public string? Room {  get; set; }

        public List<Region> Regions { get; set; }
        public string? AdminNotes {  get; set; }
    }
}
