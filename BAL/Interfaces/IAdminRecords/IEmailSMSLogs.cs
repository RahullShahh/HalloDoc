using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.IAdminRecords
{
    public interface IEmailSMSLogs
    {
        public List<EmailLogViewModel> GetEmailLogsData(string ReceiverName, string Email, DateTime? CreatedDate, DateTime? SentDate, int RoleId);

        public List<SMSLogViewModel> GetSMSLogsData(string ReceiverName, string PhoneNo, DateTime? CreatedDate, DateTime? SentDate, int RoleId);

        public EmailLogViewModel GetRolesEmailLogTable();

        public SMSLogViewModel GetRolesSMSLogTable();

    }
}
