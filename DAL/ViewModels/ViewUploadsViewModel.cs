using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ViewUploadsViewModel
    {
        public string Patientname {  get; set; }
        public string ConfirmationNo {  get; set; }
        public int RequestID { get; set; }
        public List<Requestwisefile> Requestwisefiles { get; set; }
        public IFormFile File { get; set;}
        //public DbSet<Requestwisefile> RequestFile { get; set; }
        public List<Request> req { get; set; }
        public string? ProviderNotes {  get; set; }
        public bool isFinalized {  get; set; }   
        public int? PhysicianId { get; set; }
    }
}
