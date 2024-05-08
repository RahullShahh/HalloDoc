using BAL.Interfaces;
using System.Net.Mail;
using System.Net;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.Extensions.Configuration;
using DAL.DataModels;

namespace BAL.Repository
{
    public class EmailServiceRepo : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public EmailServiceRepo(ApplicationDbContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public void SendEmailForPasswordReset(ForgotPasswordViewModel fvm,string ResetLink)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailCredentials:EmailId"]),
                Subject = "Subject",
                Body = "<h1>Hello , Good morning!!</h1><a href=\"" + ResetLink + "\" >Reset your password</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(fvm.Email);
            smtpClient.Send(mailMessage);

            Emaillog newEmailLog = new()
            {
                Subjectname = mailMessage.Subject,
                Emailid = fvm.Email,
                Createdate = DateTime.Now,
                Emailtemplate = "a",
                Roleid = 1
            };
            _context.Emaillogs.Add(newEmailLog);
            _context.SaveChanges();
        }

        public void SendEmailWithAttachments(int requestid,string path)
        {

            //var smtpClient = new SmtpClient("smtp.office365.com")
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false
            //};
            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress(_config["EmailCredentials:EmailId"]),
            //    Subject = "Subject",
            //    Body = "<p> Hello, All selected attachments are listed below!!! </p> ",
            //    IsBodyHtml = true
            //};

            //List<Attachment> files = new List<Attachment>();

            //    foreach (var filename in )
            //    {
            //        string NewFiles = Path.Combine(path, "Content", filename);
            //        var attach = new Attachment(path);
            //        files.Add(attach);
            //    }

            //    var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
            //for (int i = 0; i < request.Count; i++)
            //{
            //    string filePath = "Content/" + request[i].Filename;
            //    string fullPath = Path.Combine(path, filePath);

            //    byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            //    MemoryStream ms = new MemoryStream(fileBytes);
            //    mailMessage.Attachments.Add(new Attachment(ms, request[i].Filename));
            //}

            //var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            //mailMessage.To.Add(user.Email);
            //smtpClient.Send(mailMessage);

            //Emaillog newEmailLog = new()
            //{
            //    Subjectname = mailMessage.Subject,
            //    Emailid = user.Email,
            //    Createdate = DateTime.Now,
            //    Emailtemplate = "a",
            //    Roleid = 1
            //};
            //_context.Emaillogs.Add(newEmailLog);
            //_context.SaveChanges();
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            //smtpClient.Send("tatva.dotnet.rahulshah@outlook.com", "rahul0810shah@gmail.com", "This is a trial email for smtpClient.", "this is token ->" + resetLink);
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailCredentials:EmailId"]),
                Subject = "Subject",
                Body = "<p> Hello, All selected attachments are listed below!!! </p> ",
                IsBodyHtml = true
            };
            var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
            for (int i = 0; i < request.Count; i++)
            {
                string filePath = "Content/" + request[i].Filename;
                string fullPath = Path.Combine(path, filePath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                MemoryStream ms = new MemoryStream(fileBytes);
                mailMessage.Attachments.Add(new Attachment(ms, request[i].Filename));
            }

            var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            mailMessage.To.Add(user.Email);
            smtpClient.Send(mailMessage);
        }

        public void SendAgreementLink(int requestid, string link,string email)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailCredentials:EmailId"]),
                Subject = "Subject",
                Body = "<p>Hello, we have attached a link to the agreement that need to accepted before moving forward to the treatment procedure.</p><a href=\"" + link + "\" >Agreement Link</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);
            Emaillog newEmailLog = new()
            {
                Subjectname = mailMessage.Subject,
                Emailid = email,
                Createdate = DateTime.Now,
                Emailtemplate = "a",
                Roleid = 1
            };
            _context.Emaillogs.Add(newEmailLog);
            _context.SaveChanges();
        }

        public void SendEmailWithLink(string firstname, string lastname, string email,string link)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailCredentials:EmailId"]),
                Subject = "Subject",
                Body = "<p>Hello, Mr/Mrs "+firstname+" "+lastname+"</p><a href=\"" + link + "\" >Go to Website</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);

            Emaillog newEmailLog = new()
            {
                Subjectname = mailMessage.Subject,
                Emailid = email,
                Createdate = DateTime.Now,
                Emailtemplate = "a",
                Roleid = 1
            };
            _context.Emaillogs.Add(newEmailLog);
            _context.SaveChanges();
        }
        
        public void SendEmailForPasswordSetup(string FirstName, string LastName, string Email,string Link)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailCredentials:EmailId"]),
                Subject = "Subject",
                Body = "<p>Hello, Mr/Mrs " + FirstName + " " + LastName + "</p><a href=\"" + Link + "\" >Set-up your account using this Link</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(Email);
            smtpClient.Send(mailMessage);

            Emaillog newEmailLog = new()
            {
                Subjectname = mailMessage.Subject,
                Emailid = Email,
                Createdate = DateTime.Now,
                Emailtemplate = "a",
                Roleid = 1
            };
            _context.Emaillogs.Add(newEmailLog);
            _context.SaveChanges();
        }

        public void SendEmailMessage(String message,String email)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailCredentials:EmailId"], _config["EmailCredentials:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailCredentials:EmailId"]),
                Subject = "Admin Wants to contact you",
                Body = "<p>Hello Provider, your admin want to contact you and has sent a message for you </p>"+message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);

            Emaillog newEmailLog = new()
            {
                Subjectname = mailMessage.Subject,
                Emailid = email,
                Createdate = DateTime.Now,
                Emailtemplate = "a",
                Roleid = 1
            };
            _context.Emaillogs.Add(newEmailLog);
            _context.SaveChanges();
        }
    }
}
