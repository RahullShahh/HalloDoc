using BAL.Interfaces.IAdminRecords;
using DAL.DataContext;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.AdminRecordsRepo
{
    public class PatientHistoryPatientRecordsRepo :IPatientHistoryPatientRecords
    {
        private readonly ApplicationDbContext _context;
        public PatientHistoryPatientRecordsRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<PatientHistoryTableViewModel> GetPatientHistoryData(string FirstName, string LastName, string Email, string PhoneNo)
        {
            var PatientHistoryList = (from user in _context.Users
                                      where (string.IsNullOrEmpty(Email) || user.Email.ToLower().Contains(Email))
                                      && (string.IsNullOrEmpty(FirstName) || user.Firstname.ToLower().Contains(FirstName))
                                      && (string.IsNullOrEmpty(LastName) || user.Lastname.ToLower().Contains(LastName))
                                      && (string.IsNullOrEmpty(PhoneNo) || user.Mobile.ToLower().Contains(PhoneNo))
                                      //&& (string.IsNullOrEmpty(patientEmail) || (rc.Email).ToLower().Contains(patientEmail))
                                      select new PatientHistoryTableViewModel
                                      {
                                          Email = user.Email,
                                          FirstName = user.Firstname,
                                          LastName = user.Lastname,
                                          Address = user.Street + " " + user.City + " " + user.State + " " + user.Zipcode,
                                          PhoneNumber = user.Mobile,
                                          UserId = user.Userid
                                      }).ToList();
            return PatientHistoryList;
        }
        public List<PatientRecordsViewModel> GetPatientRecordsData(int Userid)
        {
            var PatientRecordsList = (from clients in _context.Requestclients
                                      join requests in _context.Requests on clients.Requestid equals requests.Requestid
                                      join doctors in _context.Physicians on requests.Physicianid equals doctors.Physicianid into phyGroup
                                      from physicians in phyGroup.DefaultIfEmpty()
                                      join files in _context.Requestwisefiles on clients.Requestid equals files.Requestid into fileGroup
                                      from fileItems in fileGroup.DefaultIfEmpty()
                                      join encounter in _context.Encounterforms on clients.Requestid equals encounter.Requestid into formGroup
                                      from encounterform in formGroup.DefaultIfEmpty()
                                      //join status in _context.Requeststatuses on requests.Status equals status.Statusid
                                      where requests.Userid == Userid
                                      select new PatientRecordsViewModel
                                      {
                                          ClientName = clients.Firstname + " " + clients.Lastname,
                                          CreatedDate = requests.Createddate,
                                          ConfirmationNo = requests.Confirmationnumber ?? "N/A",
                                          ProviderName = physicians.Firstname + " " + physicians.Lastname ?? "N/A",
                                          ConcludedDate = requests.Lastwellnessdate,
                                          //Status = status.Name
                                          Requestid = requests.Requestid
                                      }).ToList();
            return PatientRecordsList;
        }

    }
}
