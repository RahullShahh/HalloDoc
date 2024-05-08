using BAL.Interfaces.IAdminRecords;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.AdminRecordsRepo
{
    public class VendorDetailsRepo : IVendorDetails
    {
        private readonly ApplicationDbContext _context;
        public VendorDetailsRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public VendorDetailsViewModel GetVendorDetails()
        {
            VendorDetailsViewModel model = new()
            {
                //model.UserName = admin.Firstname + " " + admin.Lastname;
                Healthprofessionaltypes = _context.Healthprofessionaltypes.ToList()
            };
            return model;
        }
        public VendorDetailsViewModel GetFilteredDataForVendors(string filterSearch, int filterProfession)
        {

            var list = from professionals in _context.Healthprofessionals
                       join types in _context.Healthprofessionaltypes on professionals.Profession equals types.Healthprofessionalid into professionGroup
                       from proType in professionGroup.DefaultIfEmpty()
                       where (string.IsNullOrEmpty(filterSearch) || professionals.Vendorname.ToLower().Contains(filterSearch.ToLower()))
                       && (filterProfession == 0 || filterProfession == proType.Healthprofessionalid)
                       && (professionals.Isdeleted != true)
                       select new VendorDetailsTableViewModel
                       {
                           profession = proType.Professionname,
                           businessName = professionals.Vendorname,
                           email = professionals.Email,
                           faxNumber = professionals.Faxnumber,
                           phone = professionals.Phonenumber,
                           businessContact = professionals.Businesscontact,
                           vendorId = professionals.Vendorid
                       };
            VendorDetailsViewModel model = new VendorDetailsViewModel();
            model.VendorsTable = list.ToList();
            return model;
        }
        public void ChangeVendorStatusToDeleted(int vendorId)
        {
            var vendor = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == vendorId);
            if (vendor != null)
            {
                vendor.Isdeleted = true;
                _context.Healthprofessionals.Update(vendor);
                _context.SaveChanges();
            }
        }
        public void AddNewBusiness(CreateUpdateVendorViewModel model)
        {
            var mobile = "+" + model.Code + "-" + model.Phone;
            var mobile1 = "+" + model.Code1 + "-" + model.Phone1;


            //int adminId = (int)HttpContext.Session.GetInt32("adminId");
            //Admin admin = _context.Admins.FirstOrDefault(u => u.Adminid == adminId);
            var region = _context.Regions.FirstOrDefault(x => x.Regionid == model.State);

            Healthprofessional profession = new Healthprofessional()
            {
                Vendorname = model.BusinessName,
                Profession = model.Type,
                Faxnumber = model.Fax,
                Phonenumber = mobile,
                Email = model.Email,
                Businesscontact = mobile1,
                Address = model.Street,
                City = model.City,
                State = region.Name,
                Zip = model.Zip,
                Regionid = model.State,
                Createddate = DateTime.Now,
            };

            _context.Healthprofessionals.Add(profession);
            _context.SaveChanges();
        }

        public CreateUpdateVendorViewModel GetBusinessDetailsForEdit(int id)
        {
            var types = _context.Healthprofessionaltypes.ToList();
            var region = _context.Regions.ToList();

            Healthprofessional vendor = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == id);

            CreateUpdateVendorViewModel model = new CreateUpdateVendorViewModel();
            model.types = types;
            model.regions = region;
            model.BusinessName = vendor.Vendorname;
            model.Type = vendor.Profession;
            model.Fax = vendor.Faxnumber;
            model.Phone = vendor.Phonenumber;
            model.Email = vendor.Email;
            model.Phone1 = vendor.Businesscontact;
            model.Street = vendor.Address;
            model.City = vendor.City;
            model.State = vendor.Regionid;
            model.Zip = vendor.Zip;
            model.Id = id;
            return model;
        }

        public CreateUpdateVendorViewModel UpdateBusinessDetails(CreateUpdateVendorViewModel model)
        {
            var mobile = "+" + model.Code + "-" + model.Phone;
            var mobile1 = "+" + model.Code1 + "-" + model.Phone1;

            var region = _context.Regions.FirstOrDefault(x => x.Regionid == model.State);

            Healthprofessional vendor = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == model.Id);
            if (vendor != null)
            {
                vendor.Vendorname = model.BusinessName;
                vendor.Profession = model.Type;
                vendor.Faxnumber = model.Fax;
                vendor.Phonenumber = mobile;
                vendor.Email = model.Email;
                vendor.Businesscontact = mobile1;
                vendor.Address = model.Street;
                vendor.City = model.City;
                vendor.State = region.Name;
                vendor.Zip = model.Zip;
                vendor.Regionid = model.State;
                vendor.Modifieddate = DateTime.Now;

                _context.Healthprofessionals.Update(vendor);
                _context.SaveChanges();
            }

            return model;
        }

    }
}
