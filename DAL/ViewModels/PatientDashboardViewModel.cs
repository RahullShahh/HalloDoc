using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PatientDashboardViewModel
    {
        public string Username { get; set; }
        public List<Request> Requests { get; set; }
        public List<int> DocumentCount { get; set; }
    }
}