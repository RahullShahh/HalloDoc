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
    public class BlockHistoryRepo:IBlockHistory
    {
        private readonly ApplicationDbContext _context;
        public BlockHistoryRepo(ApplicationDbContext context) 
        {
            _context= context;
        }   
        public List<BlockHistoryViewModel> GetBlockHistoryData(string FirstName, string LastName, string Email, string PhoneNo)
        {
            var BlockHistoryRecords = (from blockedRequests in _context.Blockrequests
                                       join patientRequests in _context.Requests
                                       on blockedRequests.Requestid equals patientRequests.Requestid
                                       join clientRequests in _context.Requestclients
                                       on blockedRequests.Requestid equals clientRequests.Requestid
                                       where blockedRequests.Isactive == true
                                       && (string.IsNullOrEmpty(Email) || clientRequests.Email.ToLower().Contains(Email))
                                       && (string.IsNullOrEmpty(FirstName) || clientRequests.Firstname.ToLower().Contains(FirstName))
                                       && (string.IsNullOrEmpty(LastName) || clientRequests.Lastname.ToLower().Contains(LastName))
                                       && (string.IsNullOrEmpty(PhoneNo) || clientRequests.Phonenumber.ToLower().Contains(PhoneNo))
                                       select new BlockHistoryViewModel
                                       {
                                           PatientName = clientRequests.Firstname + " " + clientRequests.Lastname,
                                           PhoneNo = clientRequests.Phonenumber ?? " ",
                                           Email = clientRequests.Email ?? " ",
                                           CreatedDate = patientRequests.Createddate,
                                           Notes = clientRequests.Notes ?? " ",
                                           IsActive = blockedRequests.Isactive ?? true,
                                           RequestId = clientRequests.Requestid
                                       }).ToList();
            return BlockHistoryRecords;

        }
        public void UnblockBlockedRequest(int requestid)
        {
            var BlockedRequest = _context.Blockrequests.FirstOrDefault(request => request.Requestid == requestid);
            if (BlockedRequest != null)
            {
                BlockedRequest.Modifieddate = DateTime.Now;
                BlockedRequest.Isactive = false;
                _context.Blockrequests.Update(BlockedRequest);

            }
            var Request = _context.Requests.FirstOrDefault(request => request.Requestid == requestid);
            if (Request != null)
            {
                Request.Status = 1;
                _context.Requests.Update(Request);
            }
            _context.SaveChanges();
        }
    }
}
