using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class BusinessModel
    {
        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public string? BusinessFirstName { get; set; }
        public string? BusinessLastName { get; set; }
        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [DataType(DataType.PhoneNumber)]
        public string? BusinessPhoneNo { get; set; }
        public string? BusinessCountryCode { get; set; }
        [Required(ErrorMessage = "Email cannot be kept empty")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string? BusinessEmail { get; set; }
        [Required(ErrorMessage ="Business Name cannot be kept empty")]
        public string? BusinessName {  get; set; }
        public int? BusinessCaseNo {  get; set; }
        public string? Symptoms {  get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public string? PatientFirstName {  get; set; }
        public string? PatientLastName { get; set;}
        public DateTime? PatientDateOfBirth{  get; set; }
        [Required]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string? PatientEmail { get; set; }
        public string? Street {  get; set; }
        public string? City {  get; set; }
        [StringLength(6)]
        public string? ZipCode {  get; set; }
        [Required(ErrorMessage = "Kindly select a state")]
        public int State {  get; set; }
        public List<Region>? Regions { get; set; }
        public string? Room {  get; set; }
        public string? PatientPhoneNo { get; set; }  
        public string? PatientCountryCode { get; set; }
    }
}
