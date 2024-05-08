using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class UserAccessModel
    {
        public List<UserListViewModel>? UserList { get; set; }
        public string? UserName { get; set; }
        public List<Aspnetrole>? Aspnetroles { get; set; }
    }
}
