using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CreateRoleViewModel
    {
        public List<Menu> AccessMenu { get; set; }
        public List<Aspnetrole> AccountType{ get; set; }
        public string RoleName {  get; set; }
    }
}
