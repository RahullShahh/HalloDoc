using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PatientSetupViewModel
    {

        public string? UserName { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [RegularExpression("(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must contain 1 capital, 1 small, 1 Special symbol and at least 8 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Enter Confirm Password")]
        [Compare(nameof(Password),ErrorMessage =("Password and confirm password must match"))]
        public string ConfirmPassword {  get; set; }
    }
}
