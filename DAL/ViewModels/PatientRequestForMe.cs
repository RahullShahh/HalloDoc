using DAL.DataModels;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels
{
    public class PatientRequestForMe
    {
        public string UserName { get; set; }
        public string? symptoms { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public int? State { get; set; }

        public string? Zipcode { get; set; }

        public string? Room { get; set; }

        public IFormFile? File { get; set; }

        public int RequestID { get; set; }
        public string code { get; set; }

        public List<Region>? regions { get; set; }
    }
}
