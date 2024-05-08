using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ListOfUsersViewModel
    {
        public string? AccountType { get; set; }
        public string? AccountPOC { get; set; }

        public string? Phone { get; set; }
        public string? Status { get; set; }
        public int? OpenRequest { get; set; }

        public int? accountType { get; set; }
        public int? id { get; set; }
    }
}

