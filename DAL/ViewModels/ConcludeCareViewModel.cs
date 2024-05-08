using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ConcludeCareViewModel
    {
        public int Requestid { get; set; }
        public bool IsFinalize { get; set; }
        public string Filename { get; set; }
        public string UploaderName { get; set; }
        public string CreatedDate { get; set; }
    }
}
