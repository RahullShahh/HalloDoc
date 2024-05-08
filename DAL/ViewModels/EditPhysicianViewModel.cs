using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class EditPhysicianViewModel
    {
        [Required(ErrorMessage ="Enter Username")]
        public string? PhysicianUsername {  get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 8 characters long.")]
        public string? PhysicianPassword { get; set; }
        public string? ResidentialRegion {  get; set; }
        public short? Status {  get; set; }
        public List<Role>? Role {  get; set; }
        public int? RoleId {  get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public string? FirstName {  get; set; }
        //[Required(ErrorMessage = "Enter Last Name")]
        //[RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid last name.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        public string? Email {  get; set; }

        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? PhoneNo {  get; set; }
        [Required(ErrorMessage ="Enter Medical Licence Number")]
        public string? MedicalLicense {  get; set; }
        [Required(ErrorMessage ="Enter NPI Number")]
        public string? NPINumber {  get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        public string? SyncEmail {  get; set; }
        public List<Region>? States { get; set; }
        public int PhysicianId {  get; set; }
        public string? Address1 {  get; set; }
        public string? Address2 {  get; set; }
        public List<string>? ListOfServicingStates {  get; set; }
        public string? City {  get; set; }
        public string? ZipCode {  get; set; }
        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? BillingPhoneNo {  get; set; }
        [Required(ErrorMessage ="Enter Business name")]
        public string? BusinessName {  get; set; }
        [Required(ErrorMessage ="Enter your webiste link")]
        public string? BusinessWebsite {  get; set; }
        public string? AdminNotes {  get; set; }
        public int? Regionid {  get; set; }
        public IFormFile? SelectPhoto {  get; set; }
        public IFormFile? SelectSignature {  get; set; }
        public IFormFile? IndependentContractAgreement {  get; set; }
        public IFormFile? BackgroundCheck { get; set; }
        public IFormFile? HIPAACompliance { get; set; }
        public IFormFile? NonDisclosureAgreement { get; set; }
        public IFormFile? LicenseDocument { get; set; }
        public List<int> SelectedRegions {  get; set; }
        public int? PhysicianRole {  get; set; }
        [Required(ErrorMessage ="Select a state")]
        public int? PhysicianState {  get; set; }
        public bool? ICAexists {  get; set; }
        public bool? BCexists {  get; set; }
        public bool? HIPAAexists {  get; set; }
        public bool? NDCexists {  get; set; }
        public bool? LDexists {  get; set; }
        public string? Signatureexists {  get; set; }
        public List<int>? PhysicianChosenLocations { get; set; }
    }
}
