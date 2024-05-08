using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interfaces;
using BAL.Interfaces.IProvider;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.FileProviders.Physical;

namespace BAL.Repository.ProviderRepo
{
    public class CreateEditProviderRepo : ICreateEditProviderRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        private readonly IPasswordHasher _passwordHasher;
        public CreateEditProviderRepo(ApplicationDbContext context, IPasswordHasher passwordHasher, INotyfService notyf)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _notyf = notyf;
        }
        public Physician AddNewPhysicianDetails(EditPhysicianViewModel Model)
        {
            string id = Guid.NewGuid().ToString();
            Aspnetuser user = new Aspnetuser();
            user.Id = id;
            user.Username = Model.PhysicianUsername;
            user.Passwordhash = _passwordHasher.GenerateSHA256(Model.PhysicianPassword);
            user.Email = Model.Email;
            user.Phonenumber = Model.PhoneNo;
            user.Createddate = DateTime.Now;
            user.Role = "Physician";

            _context.Aspnetusers.Add(user);
            _context.SaveChanges();

            Physician Doctor = new Physician();
            Doctor.Aspnetuserid = id;
            Doctor.Firstname = Model.FirstName;
            Doctor.Lastname = Model.LastName;
            Doctor.Email = Model.Email;
            Doctor.Mobile = Model.PhoneNo;
            Doctor.Medicallicense = Model.MedicalLicense;
            Doctor.Adminnotes = Model.AdminNotes;
            Doctor.Address1 = Model.Address1;
            Doctor.Address2 = Model.Address2;
            Doctor.City = Model.City;
            Doctor.Zip = Model.ZipCode;
            Doctor.Altphone = Model.PhoneNo;
            Doctor.Npinumber = Model.NPINumber;
            Doctor.Medicallicense = Model.MedicalLicense;
            Doctor.Businessname = Model.BusinessName;
            Doctor.Businesswebsite = Model.BusinessWebsite;
            Doctor.Syncemailaddress = Model.SyncEmail;
            Doctor.Regionid = Model.Regionid;
            Doctor.Roleid = Model.PhysicianRole;
            Doctor.Createdby = id;
            Doctor.Regionid = Model.PhysicianState;

            _context.Physicians.Add(Doctor);
            _context.SaveChanges();

            Physiciannotification notifications = new Physiciannotification();
            notifications.Physicianid = Doctor.Physicianid;
            notifications.Isnotificationstopped = false;

            _context.Physiciannotifications.Add(notifications);
            _context.SaveChanges();

            return Doctor;
        }
        public EditPhysicianViewModel GetPhysicianDetailsForEdit(int PhysicianId)
        {
            var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == PhysicianId);
            var PhysicianAspData = _context.Aspnetusers.FirstOrDefault(x => x.Id == Physician.Aspnetuserid);
            EditPhysicianViewModel EditPhysician = new EditPhysicianViewModel();
            if (Physician != null)
            {

                EditPhysician.PhysicianId = PhysicianId;
                EditPhysician.PhoneNo = Physician.Mobile;
                EditPhysician.Status = Physician.Status;
                EditPhysician.RoleId = Physician.Roleid;
                EditPhysician.Email = Physician.Email;
                EditPhysician.FirstName = Physician.Firstname;
                EditPhysician.LastName = Physician.Lastname;
                EditPhysician.MedicalLicense = Physician.Medicallicense;
                EditPhysician.NPINumber = Physician.Npinumber;
                EditPhysician.SyncEmail = Physician.Syncemailaddress;
                EditPhysician.Address1 = Physician.Address1;
                EditPhysician.Address2 = Physician.Address2;
                EditPhysician.City = Physician.City;
                EditPhysician.States = _context.Regions.ToList();
                EditPhysician.ZipCode = Physician.Zip;
                EditPhysician.BillingPhoneNo = Physician.Altphone;
                EditPhysician.BusinessName = Physician.Businessname;
                EditPhysician.BusinessWebsite = Physician.Businesswebsite;
                EditPhysician.PhysicianUsername = PhysicianAspData.Username;
                EditPhysician.PhysicianChosenLocations =  _context.Physicianregions.Where(region => region.Physicianid==Physician.Physicianid ).ToList().Select(x => x.Regionid ).ToList();
                EditPhysician.NDCexists = Physician.Isnondisclosuredoc;
                EditPhysician.LDexists = Physician.Islicensedoc;
                EditPhysician.BCexists = Physician.Isbackgrounddoc;
                EditPhysician.ICAexists = Physician.Isagreementdoc;
                EditPhysician.HIPAAexists = Physician.Istrainingdoc;
            }

            return EditPhysician;
        }
        public EditPhysicianViewModel ProviderDashboardGetPhysicianDetailsForEditPro(int PhysicianId)
        {
            var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == PhysicianId);
            var PhysicianAspData = _context.Aspnetusers.FirstOrDefault(x => x.Id == Physician.Aspnetuserid);
            var PhyRegions = _context.Physicianregions.Where(x => x.Physicianid == PhysicianId).ToList();

            EditPhysicianViewModel EditPhysician = new EditPhysicianViewModel();
            if (Physician != null)
            {
                EditPhysician.ICAexists = Physician.Isagreementdoc;
                EditPhysician.BCexists = Physician.Isbackgrounddoc;
                EditPhysician.HIPAAexists = Physician.Istrainingdoc;
                EditPhysician.NDCexists = Physician.Isnondisclosuredoc;
                EditPhysician.LDexists = Physician.Islicensedoc;
                EditPhysician.Signatureexists = Physician.Signature;
                EditPhysician.PhysicianId = PhysicianId;
                EditPhysician.PhoneNo = Physician.Mobile;
                EditPhysician.Status = Physician.Status;
                EditPhysician.RoleId = Physician.Roleid;
                EditPhysician.Email = Physician.Email;
                EditPhysician.FirstName = Physician.Firstname;
                EditPhysician.LastName = Physician.Lastname;
                EditPhysician.MedicalLicense = Physician.Medicallicense;
                EditPhysician.NPINumber = Physician.Npinumber;
                EditPhysician.SyncEmail = Physician.Syncemailaddress;
                EditPhysician.Address1 = Physician.Address1;
                EditPhysician.Address2 = Physician.Address2;
                EditPhysician.City = Physician.City;
                EditPhysician.ZipCode = Physician.Zip;
                EditPhysician.BillingPhoneNo = Physician.Altphone;
                EditPhysician.BusinessName = Physician.Businessname;
                EditPhysician.BusinessWebsite = Physician.Businesswebsite;
                EditPhysician.PhysicianUsername = PhysicianAspData.Username;
                EditPhysician.ResidentialRegion = _context.Regions.FirstOrDefault(x => x.Regionid == Physician.Regionid).Name;
                EditPhysician.PhysicianChosenLocations = _context.Physicianregions.Where(region => region.Physicianid== Physician.Physicianid).ToList().Select(x => x.Regionid).ToList();
            }
            List<Region> phyRegions = new List<Region>();
            for (var i = 0; i < PhyRegions.Count; i++)
            {
                phyRegions.Add( _context.Regions.FirstOrDefault(x => x.Regionid == PhyRegions[i].Regionid));
            }
            EditPhysician.States = phyRegions;

            return EditPhysician;
        }
    }
}
