using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PartialTablesViewModel
    {
        public string name {  get; set; }
        public DateOnly DOB {  get; set; }
        public string requestor {  get; set; }
        public DateOnly requestedDate { get; set; }
        public string phoneNo {  get; set; }
        public string address {  get; set; }
        public string notes {  get; set; }
        public string chatWith {  get; set; }
        public string actions {  get; set; }
        public string physicianName {  get; set; }
        public DateOnly servicedate { get; set; }
        public string region {  get; set; }
    }
}
