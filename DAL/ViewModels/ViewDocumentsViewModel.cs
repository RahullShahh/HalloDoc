using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ViewDocumentsViewModel
    {
        public string Username { get; set; }
        public string ConfirmationNo { get; set; }
        public int RequestID { get; set; }
        public List<Requestwisefile> Requestwisefiles { get; set; }
        public IFormFile File { get; set; }
    }
}
