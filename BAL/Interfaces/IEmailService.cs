using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IEmailService
    {
        public void SendEmailWithAttachments(int requestid, string path);
        public void SendAgreementLink(int requestid, string link,string email);
        public void SendEmailForPasswordReset(ForgotPasswordViewModel fvm,string ResetLink);
        public void SendEmailWithLink(string firstname, string lastname, string email,string link);
        public void SendEmailMessage(String message, String email);
        public void SendEmailForPasswordSetup(string FirstName, string LastName, string Email, string Link);

    }
}
