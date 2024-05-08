using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentDataLinkLayer.ViewModels
{
    public class BookRecordsViewModel
    {
        public int BookId {  get; set; }
        public string BookName {  get; set; }
        public string? Author {  get; set; }
        public string BorrowerName {  get; set; }
        public DateTime DateOfIssue { get; set;}
        public string? City { get; set;}
        public string? Genre{ get; set;}
    }
}
