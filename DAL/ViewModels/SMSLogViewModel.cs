using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class SMSLogViewModel
    {
        public string Recipient { get; set; }
        public string Action { get; set; }
        public int? RoleId { get; set; }
        public string MoblieNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? SentDate { get; set; }
        public bool Sent { get; set; }
        public int SentTries { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<Role> roles { get; set; }
    }
}
