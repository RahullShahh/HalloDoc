using BAL.Interfaces.IAdminRecords;
using DAL.DataContext;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.AdminRecordsRepo
{
    public class SearchRecordsRepo:ISearchRecords
    {
        private readonly ApplicationDbContext _context;
        public SearchRecordsRepo(ApplicationDbContext context) 
        {
            _context=context;
        }  
        public List<SearchRecordsTableViewModel> GetPatientDataForSearchRecords(int requestStatus, string patientName, int requestType, string phoneNumber, DateTime? fromDateOfService, DateTime? toDateOfService, string providerName, string patientEmail)
        {
            var PatientRecords = (from r in _context.Requests
                                  join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                  join rs in _context.Requeststatuses on r.Status equals rs.Statusid
                                  //join rn in _context.Requestnotes on r.Requestid equals rn.Requestid
                                  join rt in _context.Requesttypes on r.Requesttypeid equals rt.Requesttypeid
                                  join phy in _context.Physicians on r.Physicianid equals phy.Physicianid into phyGroup
                                  from phyItem in phyGroup.DefaultIfEmpty()
                                  where (requestStatus == 0 || r.Status == requestStatus)
                                  && (string.IsNullOrEmpty(patientName) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(patientName.ToLower()))
                                  && (requestType == 0 || r.Requesttypeid == requestType)
                                  && (r.Accepteddate >= fromDateOfService || fromDateOfService == null)
                                  && (r.Accepteddate <= toDateOfService || toDateOfService == null)
                                  && (string.IsNullOrEmpty(providerName) || (r.Physician.Firstname + " " + r.Physician.Lastname).ToLower().Contains(providerName.ToLower()))
                                  && (string.IsNullOrEmpty(patientEmail) || (rc.Email).ToLower().Contains(patientEmail))
                                  && (string.IsNullOrEmpty(phoneNumber) || (rc.Phonenumber).ToLower().Contains(phoneNumber))
                                  && (r.Isdeleted!=true)
                                  select new SearchRecordsTableViewModel
                                  {
                                      PatientName = rc.Firstname + " " + rc.Lastname,
                                      Requestor = r.Firstname + " " + r.Lastname,
                                      DateOfService = DateOnly.FromDateTime(DateTime.Now),
                                      CloseCaseDate = DateOnly.FromDateTime(DateTime.Now),
                                      Email = rc.Email,
                                      PhoneNumber = rc.Phonenumber,
                                      Address = rc.Address,
                                      Zip = rc.Zipcode,
                                      RequestStatus = rs.Name,
                                      //PhysicianNotes = rn.Physiciannotes,
                                      //AdminNotes = rn.Adminnotes,
                                      PatientNotes = "PatientNotes",
                                      PhysicianName = phyItem.Firstname + " " + phyItem.Lastname,
                                      CancelledByPhysicianNotes = "N/A",
                                      RequestId=rc.Requestid
                                  }).ToList();
            return PatientRecords;
        }

        public SearchRecordViewModel GetSearchRecordsData()
        {
            SearchRecordViewModel model = new SearchRecordViewModel()
            {
                RequestStatus = _context.Requeststatuses.ToList(),
                RequestType = _context.Requesttypes.ToList()
            };
            return model;
        }


    }
}
