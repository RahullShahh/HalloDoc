using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Enter Your Username")]
        public required string Username {  get; set; }
        [Required(ErrorMessage ="Enter Password")]
        public required string Password { get; set; }
    }
}