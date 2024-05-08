using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class PatientDashboardRepo : IPatientDashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileOperations _fileOperations;
        public PatientDashboardRepo(ApplicationDbContext context, IFileOperations fileOperations)
        {
            _context = context;
            _fileOperations = fileOperations;
        }

        public static string GetDOB(Requestclient reqcli)
        {
            string dob = reqcli.Intyear + "-" + reqcli.Strmonth + "-" + reqcli.Intdate;
            if (reqcli.Intyear == null || reqcli.Strmonth == null || reqcli.Intdate == null)
            {
                return " ";
            }

            string dobdate = DateTime.Parse(dob).ToString("MMM dd, yyyy");

            return dobdate;
        }

        public PatientProfileViewModel PatientProfile(string email)
        {
            User v = _context.Users.FirstOrDefault(dt => dt.Email == email);
            Requestclient patient=_context.Requestclients.FirstOrDefault(dt => dt.Email == email);
            PatientProfileViewModel ppm = new();
            if (v != null)
            {

                ppm.FirstName = v.Firstname;
                ppm.LastName = v.Lastname ?? "";
                ppm.PhoneNo = v.Mobile ?? "";
                ppm.zipcode = v.Zipcode ?? "";
                ppm.street = v.Street ?? "";
                ppm.userid = v.Userid;
                ppm.state = v.State ?? "";
                ppm.email = v.Email;
                ppm.city = v.City ?? "";
                //ppm.Date = GetDOB(patient);
            }
            return ppm;
        }
        public void EditProfile(PatientProfileViewModel ppm, string email)
        {
            User dbUser = _context.Users.FirstOrDefault(u => u.Email == ppm.email);
            if (dbUser != null)
            {
                dbUser.Firstname = ppm.FirstName;
                dbUser.Lastname = ppm.LastName;
                dbUser.Intdate = ppm.Date.Day;
                dbUser.Strmonth = ppm.Date.Month.ToString();
                dbUser.Intyear = ppm.Date.Year;
                dbUser.Mobile = ppm.PhoneNo;
                dbUser.Street = ppm.street;
                dbUser.City = ppm.city;
                dbUser.State = ppm.state;
                dbUser.Zipcode = ppm.zipcode;
                _context.Update(dbUser);
            }
        }

        public ViewDocumentsViewModel ViewPatientDocsGet(int requestid, string email)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == email);
            Request request = _context.Requests.FirstOrDefault(v => v.Requestid == requestid);
            List<Requestwisefile> files = _context.Requestwisefiles.Where(reqFiles => reqFiles.Requestid == requestid).ToList();
            ViewDocumentsViewModel vm = new ViewDocumentsViewModel();
            vm.ConfirmationNo = request.Confirmationnumber;
            vm.RequestID = requestid;
            vm.Username = user.Firstname + " " + user.Lastname;
            vm.Requestwisefiles = files;
            return vm;
        }
        public ViewDocumentsViewModel ViewPatientDocsPost(ViewDocumentsViewModel vm, string path)
        {
            if (vm.File != null)
            {
                _fileOperations.insertfiles(vm.File, path);
                Requestwisefile rwf = new()
                {
                    Requestid = vm.RequestID,
                    Filename = vm.File.FileName,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(rwf);
                _context.SaveChanges();
            }
            return vm;
        }
        public PatientDashboardViewModel PatientDashboard(string email)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return null;
            }
            PatientDashboardViewModel pd = new()
            {
                Username = user.Firstname + " " + user.Lastname,
                Requests = _context.Requests.Where(req => req.Userid == user.Userid).ToList()
            };
            List<int> documentCount = new List<int>();
            for (int i = 0; i < pd.Requests.Count; i++)
            {
                int count = _context.Requestwisefiles.Count(rf => rf.Requestid == pd.Requests[i].Requestid);
                documentCount.Add(count);
            }
            pd.DocumentCount = documentCount;
            return pd;
        }
    }
}
