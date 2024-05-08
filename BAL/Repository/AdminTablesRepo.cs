using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class AdminTablesRepo : IAdminTables
    {
        private readonly ApplicationDbContext _context;
        public AdminTablesRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public static string GetPatientDOB(Requestclient u)
        {

            string udb = u.Intyear + "-" + u.Strmonth + "-" + u.Intdate;
            if (u.Intyear == null || u.Strmonth == null || u.Intdate == null)
            {
                return "";
            }

            DateTime dobDate = DateTime.Parse(udb);
            string dob = dobDate.ToString("MMM dd, yyyy");
            var today = DateTime.Today;
            var age = today.Year - dobDate.Year;
            if (dobDate.Date > today.AddYears(-age)) age--;

            string dobString = dob + " (" + age + ")";

            return dobString;
        }
        public AdminDashboardViewModel AdminDashboardView()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     email = rc.Email
                                 }).ToList();
            AdminRequestsViewModel arvm = new AdminRequestsViewModel();
            AdminDashboardViewModel advm = new()
            {
                adminRequests = adminRequests,
                Username = arvm.Name,
            };
            return advm;
        }
        public AdminDashboardViewModel GetActiveTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }

            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                  && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     isFinalize = _context.Encounterforms.Where(x => x.Requestid == r.Requestid).Select(x => x.Isfinalize).FirstOrDefault(),
                                     DateOfBirth = GetPatientDOB(rc),
                                     email = rc.Email,
                                     PhysicianId = r.Physicianid,
                                     Notes = r.Requeststatuslogs.OrderByDescending(r => r.Createddate).FirstOrDefault().Notes
                                 }).Where(x => x.status == 4 || x.status == 5).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetConcludeTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email,
                                     isFinalize = _context.Encounterforms.Where(x => x.Requestid == r.Requestid).Select(x => x.Isfinalize).FirstOrDefault(),
                                     DateOfBirth=GetPatientDOB(rc),
                                     PhysicianId=r.Physicianid
                                 }
                               ).Where(x => x.status == 6).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetNewTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     email=rc.Email,
                                     DateOfBirth=GetPatientDOB(rc),
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     PhysicianId = r.Physicianid
                                 }).Where(x => x.status == 1).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
                TotalPage = (int)Math.Ceiling(adminRequests.Count() / (double)pagesize)
            };
            return model;
        }
        public AdminDashboardViewModel GetPendingTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email,
                                     DateOfBirth=GetPatientDOB(rc),
                                     PhysicianId = r.Physicianid

                                 }).Where(x => x.status == 2).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetToCloseTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email,
                                     Notes = _context.Requeststatuslogs.Where(log => log.Requestid == r.Requestid).OrderByDescending(_ => _.Createddate).First().Notes,
                                     PhysicianId = r.Physicianid,
                                     DateOfBirth = GetPatientDOB(rc)
                                 }).Where(x => x.status == 3 || x.status == 7 || x.status == 8).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();

            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetUnpaidTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email,
                                     Notes = _context.Requeststatuslogs.Where(log => log.Requestid == r.Requestid).OrderByDescending( _=> _.Createddate).First().Notes,
                                     DateOfBirth = GetPatientDOB(rc),
                                     PhysicianId = r.Physicianid
                                 }).Where(x => x.status == 9).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel AdminDashboard(string email)
        {
            List<Physician> physician = _context.Physicians.ToList();
            List<Region> regions = _context.Regions.ToList();
            List<Casetag> casetags = _context.Casetags.ToList();

            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);

            AdminDashboardViewModel advm = new()
            {
                physician = physician,
                regions = regions,
                casetags = casetags,
                New = _context.Requests.Count(u => u.Status == 1),
                active = _context.Requests.Count(u => u.Status == 4 || u.Status == 5),
                pending = _context.Requests.Count(u => u.Status == 2),
                conclude = _context.Requests.Count(u => u.Status == 6),
                toclose = _context.Requests.Count(u => u.Status == 7 || u.Status == 3 || u.Status == 8),
                unpaid = _context.Requests.Count(u => u.Status == 9),
                Username = admin.Firstname + " " + admin.Lastname
            };
            return advm;
        }



        public AdminDashboardViewModel ProviderDashboard(string aspnetuserid)
        {
            List<Physician> physician = _context.Physicians.ToList();
            List<Region> regions = _context.Regions.ToList();
            List<Casetag> casetags = _context.Casetags.ToList();

            var doctor = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == aspnetuserid);

            AdminDashboardViewModel advm = new()
            {
                physician = physician,
                regions = regions,
                casetags = casetags,
                New = _context.Requests.Where(phy=>phy.Physicianid==doctor.Physicianid).Count(u => u.Status == 1),
                active = _context.Requests.Where(phy => phy.Physicianid == doctor.Physicianid).Count(u => u.Status == 4 || u.Status == 5),
                pending = _context.Requests.Where(phy => phy.Physicianid == doctor.Physicianid).Count(u => u.Status == 2),
                conclude = _context.Requests.Where(phy => phy.Physicianid == doctor.Physicianid).Count(u => u.Status == 6),
                toclose = _context.Requests.Where(phy => phy.Physicianid == doctor.Physicianid).Count(u => u.Status == 7 || u.Status == 3 || u.Status == 8),
                unpaid = _context.Requests.Where(phy => phy.Physicianid == doctor.Physicianid).Count(u => u.Status == 9),
                Username = doctor.Firstname + " " + doctor.Lastname
            };
            return advm;
        }
        public AdminDashboardViewModel ProviderActiveTable(DashboardFilter filter,int physicianid)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }

            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                  && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 && (r.Physicianid == physicianid)
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     isFinalize = _context.Encounterforms.Where(x => x.Requestid == r.Requestid).Select(x => x.Isfinalize).FirstOrDefault(),
                                     DateOfBirth = GetPatientDOB(rc),
                                     email = rc.Email,
                                     PhysicianId = r.Physicianid,
                                     calltype=r.Calltype

                                 }).Where(x => x.status == 4 || x.status == 5).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();

            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel ProviderConcludeTable(DashboardFilter filter,int physicianid)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 && (r.Physicianid == physicianid)
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email,
                                     isFinalize = _context.Encounterforms.Where(x => x.Requestid == r.Requestid).Select(x => x.Isfinalize).FirstOrDefault(),
                                     DateOfBirth = GetPatientDOB(rc),
                                     PhysicianId = r.Physicianid,
                                     calltype = r.Calltype
                                 }
                               ).Where(x => x.status == 6).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel ProviderNewTable(DashboardFilter filter,int physicainid)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 && (r.Physicianid == physicainid)
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     email = rc.Email,
                                     DateOfBirth = GetPatientDOB(rc),
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     PhysicianId = r.Physicianid
                                 }).Where(x => x.status == 1).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
                TotalPage = (int)Math.Ceiling(adminRequests.Count() / (double)pagesize)
            };
            return model;
        }
        public AdminDashboardViewModel ProviderPendingTable(DashboardFilter filter,int physicainid)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into DoctorsGroup
                                 from Doctor in DoctorsGroup.DefaultIfEmpty()
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 && (r.Physicianid==physicainid)
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = Doctor.Firstname + " " + Doctor.Lastname,
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email,
                                     DateOfBirth = GetPatientDOB(rc),
                                     PhysicianId = r.Physicianid

                                 }).Where(x => x.status == 2).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
    }
}

