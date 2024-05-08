using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CreateUpdateVendorViewModel
    {
        public string? UserName { get; set; }
        public List<Healthprofessionaltype>? types { get; set; }
        public List<Region>? regions { get; set; }
        [Required(ErrorMessage ="Select Business name")]
        public string? BusinessName { get; set; }
        [Required(ErrorMessage ="Select type of profession")]
        public int? Type { get; set; }
        public string? Fax { get; set; }
        public string? Code { get; set; }
        [Required(ErrorMessage ="Enter your phone number.")]
        public string? Phone { get; set; }
        [Required(ErrorMessage ="Enter email-id")]
        public string? Email { get; set; }
        public string? Code1 { get; set; }
        [Required(ErrorMessage = "Enter your alternate phone number.")]
        public string? Phone1 { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        [Required(ErrorMessage ="Select Business Region")]
        public int? State { get; set; }
        [StringLength(6)]
        public string? Zip { get; set; }
        public int? Id { get; set; }
    }
}
