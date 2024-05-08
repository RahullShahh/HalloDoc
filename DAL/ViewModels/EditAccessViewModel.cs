using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class EditAccessViewModel
    {
        //public string? UserName { get; set; }
        public int? id { get; set; }
        public string? role { get; set; }
        public int? type { get; set; }
        public List<Aspnetrole>? accountTypes { get; set; }
        public List<int> rolemenu { get; set; }
        public List<Menu> menu { get; set; }
    }
}
