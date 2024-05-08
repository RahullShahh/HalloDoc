using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CloseCaseViewModel
    {
        public List<Requestwisefile> RequestwisefileList;
        public string firstname {  get; set; }
        public string lastname { get; set; }
        public DateTime dateofbirth { get; set; }
        public string phoneno {  get; set; }
        public string email {  get; set; }
        public string confirmationNo {  get; set; }
        public int requestid {  get; set; }
    }
}
