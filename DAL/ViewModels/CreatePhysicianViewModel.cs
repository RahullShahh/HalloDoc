using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CreatePhysicianViewModel
    {
        public string Username { get; set; }
        public string? Password { get; set; }
        public List<Role> Role { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string MedicalLicenseNo { get; set; }
        public string NPINumber { get; set; }
        public List<Region>? regions { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City {  get; set; }
        public int? State {  get; set; }
        public int? ZipCode {  get; set; }
        public string? BillingPhoneNo {  get; set; }
        public string? BusinessName {  get; set; }
        public string? BusinessWebsite {  get; set; }
        public IFormFile? FormFile { get; set; }
        public string? AdminNotes {  get; set; }
        public IFormFile? IndependentContractorAgreement { get; set; }
        public IFormFile? BackgroundCheck {  get; set; }
        public IFormFile? HIPPACompliance { get; set; }
        public IFormFile? NonDisclosureAgreement { get; set; }
    }
}
