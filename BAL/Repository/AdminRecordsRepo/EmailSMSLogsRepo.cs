using BAL.Interfaces.IAdminRecords;
using DAL.DataContext;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.AdminRecordsRepo
{
    public class EmailSMSLogsRepo : IEmailSMSLogs
    {
        private readonly ApplicationDbContext _context;
        public EmailSMSLogsRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<EmailLogViewModel> GetEmailLogsData(string ReceiverName, string Email, DateTime? CreatedDate, DateTime? SentDate, int RoleId)
        {
            var EmailList = (from emails in _context.Emaillogs
                             join roles in _context.Roles on emails.Roleid equals roles.Roleid
                             where (string.IsNullOrEmpty(ReceiverName) /*|| emails.Recipient.ToLower().Contains(ReceiverName)*/)
                             && (string.IsNullOrEmpty(Email) || emails.Emailid.ToLower().Contains(Email))
                             && (CreatedDate == emails.Createdate.Date || CreatedDate == null)
                             && (SentDate == emails.Sentdate.Value.Date || SentDate == null)
                             && (RoleId == 0 || RoleId == emails.Roleid)
                             select new EmailLogViewModel
                             {
                                 Action = emails.Subjectname,
                                 RoleId = emails.Roleid,
                                 Email = emails.Emailid,
                                 CreateDate = emails.Createdate,
                                 SentDate = emails.Sentdate,
                                 Sent = false,
                                 SentTries = 1,
                                 ConfirmationNumber = emails.Confirmationnumber ?? "n/a"
                             }).ToList();
            return EmailList;
        }

        public EmailLogViewModel GetRolesEmailLogTable()
        {
            EmailLogViewModel emaildata = new()
            {
                roles = _context.Roles.ToList()
            };
            return emaildata;

        }

        public SMSLogViewModel GetRolesSMSLogTable()
        {
            SMSLogViewModel SMSdata = new()
            {
                roles = _context.Roles.ToList()
            };
            return SMSdata;
        }

        public List<SMSLogViewModel> GetSMSLogsData(string ReceiverName, string PhoneNo, DateTime? CreatedDate, DateTime? SentDate, int RoleId)
        {
            var SMSList = (from sms in _context.Smslogs
                           join roles in _context.Roles on sms.Roleid equals roles.Roleid
                           where (string.IsNullOrEmpty(ReceiverName) /*|| emails.Recipient.ToLower().Contains(ReceiverName)*/)
                           && (string.IsNullOrEmpty(PhoneNo) || sms.Mobilenumber.ToLower().Contains(PhoneNo))
                           && (CreatedDate == sms.Createdate.Date || CreatedDate == null)
                           && (SentDate == sms.Sentdate.Value.Date || SentDate == null)
                           && (RoleId == 0 || RoleId == sms.Roleid)
                           select new SMSLogViewModel
                           {
                               Action = sms.Action.ToString(),
                               RoleId = sms.Roleid,
                               MoblieNumber = sms.Mobilenumber,
                               CreateDate = sms.Createdate,
                               SentDate = sms.Sentdate,
                               Sent = false,
                               SentTries = 1,
                               ConfirmationNumber = sms.Confirmationnumber ?? "n/a"
                           }).ToList();
            return SMSList;
        }
    }
}
