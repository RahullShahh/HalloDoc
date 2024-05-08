using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace BAL.Repository
{
    public enum RequestStatus
    {
        Unassigned = 1,
        Accepted = 2,
        Cancelled = 3,
        MDEnRoute = 4,
        MDOnSite = 5,
        Conclude = 6,
        CancelledByPatient = 7,
        Closed = 8,
        Unpaid = 9,
        Clear = 10,
        Block = 11,
    }
    public class AdminActionsRepo : IAdminActions
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        public AdminActionsRepo(ApplicationDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        public ViewCaseViewModel ViewCaseAction(int requestid)
        {
            Requestclient rc = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);
            ViewCaseViewModel vc = new()
            {
                requestID = rc.Requestid,
                patientemail = rc.Email,
                patientfirstname = rc.Firstname,
                patientlastname = rc.Lastname,
                patientnotes = rc.Notes,
                patientphone = rc.Phonenumber,
                address = rc.Address,
                rooms = "N/A"
            };
            return vc;
        }
        public void AssignCaseAction(int RequestId, string AssignPhysician, string AssignDescription)
        {
            Request user = _context.Requests.FirstOrDefault(h => h.Requestid == RequestId);
            if (user != null)
            {
                user.Modifieddate = DateTime.Now;
                user.Physicianid = int.Parse(AssignPhysician);
                _context.Requests.Update(user);

                Requeststatuslog requeststatuslog = new Requeststatuslog();
                requeststatuslog.Requestid = RequestId;
                requeststatuslog.Notes = "Approved by admin";
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 1;
                _context.Add(requeststatuslog);
                _context.SaveChanges();
            }
        }
        public void ProviderAcceptCase(int requestid)
        {
            Request user = _context.Requests.FirstOrDefault(h => h.Requestid == requestid);
            user.Status = 2;
            user.Modifieddate = DateTime.Now;
            _context.Requests.Update(user);

            Requeststatuslog requeststatuslog = new Requeststatuslog();
            requeststatuslog.Requestid = requestid;
            requeststatuslog.Notes = "Accepted by physician";
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Status = 2;
            _context.Add(requeststatuslog);
            _context.SaveChanges();
        }
        public void CancelCaseAction(int requestid, string Reason, string Description)
        {
            var user = _context.Requests.FirstOrDefault(h => h.Requestid == requestid);
            if (user != null)
            {
                user.Status = 3;
                user.Casetag = Reason;

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = requestid;
                requeststatuslog.Notes = Description;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 3;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                _context.Update(user);
                _context.SaveChanges();
            }
        }

        public List<Region> GetRegionsList()
        {
            var regions = _context.Regions.ToList();
            return regions;
        }

        public List<RequestedShiftsViewModel> GetRequestedShifts(string regionid)
        {
            var list = (
                         from shiftdetail in _context.Shiftdetails
                         where shiftdetail.Status == 0 && shiftdetail.Isdeleted != true
                         orderby shiftdetail.Shiftdate
                         select new RequestedShiftsViewModel()
                         {
                             ShiftDetailId = shiftdetail.Shiftdetailid,
                             Staff = string.Concat(shiftdetail.Shift.Physician.Firstname, " ", shiftdetail.Shift.Physician.Lastname ?? ""),
                             Day = shiftdetail.Shiftdate.ToString("MMM dd,yyyy"),
                             Time = string.Concat(shiftdetail.Starttime.ToString("h:mmtt"), "-", shiftdetail.Endtime.ToString("h:mmtt")),
                             Regionid = (int)(shiftdetail.Regionid != null ? shiftdetail.Regionid : 0),
                             Region = _context.Regions.FirstOrDefault(r => r.Regionid == shiftdetail.Regionid).Name ?? "",
                         }
                       ).Where(x => (string.IsNullOrEmpty(regionid)) || (x.Regionid == int.Parse(regionid))).ToList();
            return list;
        }

        public List<EventsViewModel> ListOfEvents()
        {
            var list = (
                          from shift in _context.Shifts
                          join shiftDetail in _context.Shiftdetails
                          on shift.Shiftid equals shiftDetail.Shiftid
                          join physician in _context.Physicians
                          on shift.Physicianid equals physician.Physicianid
                          where shiftDetail.Isdeleted != true
                          select new EventsViewModel()
                          {
                              Id = shiftDetail.Shiftdetailid,
                              ResourceId = shift.Physicianid,
                              Title = string.Concat(shiftDetail.Starttime.ToString("h:mmtt"), "-", shiftDetail.Endtime.ToString("h:mmtt"), "/", physician.Lastname ?? "", " ", physician.Firstname.Substring(0, 1)),
                              StartTime = (new DateTime(shiftDetail.Shiftdate.Year, shiftDetail.Shiftdate.Month, shiftDetail.Shiftdate.Day, shiftDetail.Starttime.Hour, shiftDetail.Starttime.Minute, shiftDetail.Starttime.Second)).ToString("s"),
                              EndTime = (new DateTime(shiftDetail.Shiftdate.Year, shiftDetail.Shiftdate.Month, shiftDetail.Shiftdate.Day, shiftDetail.Endtime.Hour, shiftDetail.Endtime.Minute, shiftDetail.Endtime.Second)).ToString("s"),
                              Status = shiftDetail.Status,
                              ShiftDetailsId = shiftDetail.Shiftdetailid,
                              Regionid = shiftDetail.Regionid,
                              Region = _context.Regions.FirstOrDefault(x => x.Regionid == shiftDetail.Regionid).Name ?? "",
                          }
                ).ToList();
            return list;
        }
        public void BlockCaseAction(int requestid, string blocknotes)
        {
            var user = _context.Requests.FirstOrDefault(u => u.Requestid == requestid);
            if (user != null)
            {
                user.Status = 11;

                _context.Update(user);
                _context.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = requestid;
                requeststatuslog.Notes = blocknotes ?? "--";
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 11;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                Blockrequest? blockRequest = _context.Blockrequests.FirstOrDefault(blockedrequest => blockedrequest.Requestid == requestid);

                if (blockRequest != null)
                {
                    blockRequest.Requestid = requestid;
                    blockRequest.Modifieddate = DateTime.Now;
                    blockRequest.Email = user.Email;
                    blockRequest.Phonenumber = user.Phonenumber;
                    blockRequest.Reason = blocknotes ?? "--";
                    blockRequest.Isactive = true;
                    _context.Blockrequests.Update(blockRequest);
                }
                else
                {
                    Blockrequest blocked = new()
                    {
                        Requestid = requestid,
                        Createddate = DateTime.Now,
                        Email = user.Email,
                        Phonenumber = user.Phonenumber,
                        Reason = blocknotes ?? "--",
                        Isactive = true
                    };
                    _context.Blockrequests.Add(blocked);
                }
                _context.SaveChanges();
            }
        }
        public void TransferCase(int RequestId, string TransferPhysician, string TransferDescription, int adminid)
        {
            var req = _context.Requests.FirstOrDefault(h => h.Requestid == RequestId);
            if (req != null)
            {
                req.Status = 2;
                req.Modifieddate = DateTime.Now;
                req.Physicianid = int.Parse(TransferPhysician);

                _context.Update(req);
                _context.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = RequestId;
                requeststatuslog.Notes = TransferDescription;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 2;
                requeststatuslog.Transtophysicianid = int.Parse(TransferPhysician);
                requeststatuslog.Adminid = adminid;

                _context.Add(requeststatuslog);
                _context.SaveChanges();
            }
        }
        public void ProviderTransferCase(int RequestId, string TransferPhysician, string TransferDescription, int ProviderId)
        {
            var req = _context.Requests.FirstOrDefault(h => h.Requestid == RequestId);
            if (req != null)
            {
                req.Status = 1;
                req.Modifieddate = DateTime.Now;
                req.Physicianid = null;
                _context.Update(req);
                _context.SaveChanges();

                Requeststatuslog requeststatuslog = new()
                {
                    Requestid = RequestId,
                    Notes = TransferDescription,
                    Createddate = DateTime.Now,
                    Status = 1,
                    Physicianid = ProviderId
                };

                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.SaveChanges();
            }
        }
        public bool ClearCaseModal(int requestid)
        {
            //Admin admin = _context.Admins.GetFirstOrDefault(a => a.Email == AdminEmail);
            try
            {
                Request req = _context.Requests.FirstOrDefault(req => req.Requestid == requestid);
                req.Modifieddate = DateTime.Now;

                Requeststatuslog reqStatusLog = new()
                {
                    Requestid = requestid,
                    Status = (short)RequestStatus.Clear,
                    //Adminid = adminid,
                    Notes = "Admin cleared this request",
                    Createddate = DateTime.Now,
                };

                req.Status = (short)RequestStatus.Clear;
                req.Modifieddate = DateTime.Now;


                _context.Requests.Update(req);
                _context.SaveChanges();

                _context.Requeststatuslogs.Add(reqStatusLog);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        void IAdminActions.SendOrderAction(int requestid, SendOrderViewModel sendOrder)
        {
            Orderdetail Order = new()
            {
                Requestid = requestid,
                Faxnumber = sendOrder.FaxNo,
                Email = sendOrder.BusEmail,
                Businesscontact = sendOrder.BusContact,
                Prescription = sendOrder.prescription,
                Noofrefill = sendOrder.RefillCount,
                Createddate = DateTime.Now,
                Vendorid = 1
            };
            _context.Orderdetails.Add(Order);
            _context.SaveChanges();
            _notyf.Success("Order Placed Successfully");

        }

        public CloseCaseViewModel CloseCaseGet(int requestid)
        {
            var files = _context.Requestwisefiles.Where(x => x.Requestid == requestid).ToList();
            Requestclient user = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);
            var patient = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            //string dob = user.Intyear + "-" + user.Strmonth.ToString() + "-" + user.Intdate;

            CloseCaseViewModel model = new()
            {
                firstname = user.Firstname??" ",
                lastname = user.Lastname??" ",
                //dateofbirth = DateTime.Parse(dob),
                phoneno = user.Phonenumber ?? " ",
                email = user.Email??"",
                RequestwisefileList = files,
                requestid = requestid,
                confirmationNo = patient.Confirmationnumber ?? " "
            };
            return model;
        }

        public void CloseCasePost(CloseCaseViewModel model, int requestid)
        {
            var user = _context.Requestclients.FirstOrDefault(r => r.Requestid == requestid);
            if (user != null)
            {
                user.Firstname = model.firstname;
                user.Lastname = model.lastname;
                user.Phonenumber = model.phoneno;
                user.Email = model.email;

                _context.Requestclients.Update(user);
            }
            _context.SaveChanges();
        }
        public void ChangeRequestStatusToClosed(int requestId)
        {
            var user = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);
            if (user != null)
            {
                user.Status = 9;
                _context.Requests.Update(user);
                _context.SaveChanges();
            }
        }
        public void CreateRequestFromAdminDashboard(CreateRequestViewModel model)
        {
            var user = _context.Requests.FirstOrDefault(x => x.Email == model.Email);
            if (user != null)
            {
                //var newvm=new PatientModel();
                Aspnetuser newUser = new Aspnetuser();

                string id = Guid.NewGuid().ToString();
                newUser.Id = id;
                newUser.Email = model.Email;
                newUser.Phonenumber = model.PhoneNo;
                newUser.Username = model.FirstName;
                newUser.Createddate = DateTime.Now;
                _context.Aspnetusers.Add(newUser);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = newUser.Id;
                user_obj.Firstname = model.FirstName;
                user_obj.Lastname = model.LastName;
                user_obj.Email = model.Email;
                user_obj.Mobile = model.PhoneNo;
                user_obj.Street = model.street;
                user_obj.City = model.City;
                user_obj.State = model.State;
                user_obj.Zipcode = model.Zipcode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                _context.Users.Add(user_obj);
                _context.SaveChanges();

                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Userid = user_obj.Userid;
                request.Firstname = model.FirstName;
                request.Lastname = model.LastName;
                request.Phonenumber = model.PhoneNo;
                request.Email = model.Email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = id;
                request.Status = 1;
                request.Createduserid = user_obj.Userid;
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = model.FirstName;
                rc.Lastname = model.LastName;
                rc.Phonenumber = model.PhoneNo;
                rc.Location = model.City + model.State;
                rc.Email = model.Email;
                rc.Address = model.street + " " + model.City + " " + model.State + " " + model.Zipcode;
                rc.Street = model.street;
                rc.City = model.City;
                rc.State = model.State;
                rc.Zipcode = model.Zipcode;
                rc.Notes = model.AdminNotes;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();
                _notyf.Success("New Request Created");
            }
            else
            {
                User user_obj = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Firstname = model.FirstName;
                request.Lastname = model.LastName;
                request.Phonenumber = model.PhoneNo;
                request.Email = model.Email;
                request.Createddate = DateTime.Now;
                request.Status = 1;
                //request.Createduserid = user_obj.Userid;
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = model.FirstName;
                rc.Lastname = model.LastName;
                rc.Phonenumber = model.PhoneNo;
                rc.Location = model.City + " " + model.State;
                rc.Email = model.Email;
                rc.Address = model.City + ", " + model.street + ", " + model.State + ", " + model.Zipcode;
                rc.Street = model.street;
                rc.City = model.City;
                rc.State = model.State;
                rc.Zipcode = model.Zipcode;
                rc.Notes = model.AdminNotes;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();
                _notyf.Success("New Request Created");

            }
        }
    }

}

