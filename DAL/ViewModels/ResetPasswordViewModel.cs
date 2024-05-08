using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string email {  get; set; }  
        public string password { get; set; }    
        public string confirmpassword {  get; set; }
    }
}
