using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.ViewModels.ConciergeModel;
namespace DAL.ViewModels
{
    public class EncounterFormViewModel
    {
        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public required string FirstName {  get; set; }
        [Required(ErrorMessage = "First name cannot be kept empty")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Please enter a valid first name.")]
        public required string LastName { get; set; }
        public string? Location {  get; set; }
        [Required(ErrorMessage ="Date of birth cannot be empty")]
        //[DateNotInFutureAttribute(ErrorMessage = "Future date cannot be selected")]
        public DateOnly DateOfBirth { get; set; }
        public DateOnly? Date {  get; set; }
        [Required(ErrorMessage = "Phone number cannot be kept empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? PhoneNo {  get; set; }
        [Required(ErrorMessage = "Email cannot be kept empty")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string? Email {  get; set; }
        public string? IllnessHistory {  get; set; }
        public string? MedicalHistory {  get; set; }
        public string? Medications {  get; set; }
        public string? Allergies {  get; set; }
        public string? Temperature {  get; set; }
        [Required(ErrorMessage ="Enter Heart Rate reading")]
        public string HR {  get; set; }
        public string? RR {  get; set; }
        [Required(ErrorMessage = "Enter BP Low Reading")]
        public string BPLow {  get; set; }
        [Required(ErrorMessage = "Enter BP High Reading")]
        public string BPHigh {  get; set; }
        [Required(ErrorMessage = "Enter blood oxygen reading")]
        public string O2 {  get; set; }
        public string? Pain { get; set; }
        public string? heent { get; set; }
        public string? cv { get; set; }
        public string? chest { get; set; }
        public string? abd { get; set; }
        public string? extr { get; set; }
        public string? skin { get; set; }
        public string? neuro { get; set; }
        public string? other { get; set; }
        public string? diagnosis { get; set; }
        public string? treatmentPlan { get; set; }
        public string? MedicationsDispensed { get; set; }
        public string? procedures { get; set; }
        public string? followUps { get; set; }
        public int requestId { get; set; }
        public bool IfExists { get; set; } 
        public int CallType {  get; set; }
    }
}
