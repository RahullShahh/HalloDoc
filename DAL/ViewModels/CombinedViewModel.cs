using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CombinedViewModel
    {

        public AdminDashboardViewModel AdminDashboardViewModel { get; set; }
        public IList<ExcelFileViewModel> ExcelFileViewModel { get; set; }

    }
}
