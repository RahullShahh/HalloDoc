using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml;

namespace BAL.Repository
{
    public class RequestRepo : IRequestRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IFileOperations _insertfiles;
        private readonly IEmailService _emailService;
        public RequestRepo(ApplicationDbContext context, IPasswordHasher passwordHasher, IFileOperations insertfiles, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _insertfiles = insertfiles;
            _emailService = emailService;
        }

        public void FRequest(FamilyFriendModel fmfr, string uniqueid, string path, string SetUpLink)
        {
            bool userExists = _context.Users.Any(RequestedPatient => RequestedPatient.Email == fmfr.PatientModel.Email);
            string PatientPhoneNumber = "+" + fmfr.PatientModel.CountryCode + "-" + fmfr.PatientModel.PhoneNo;
            string FriendFamilyPhoneNo = "+" + fmfr.FriendFamilyCountryCode + "-" + fmfr.FriendFamilyPhoneNo;
            var StateName = _context.Regions.FirstOrDefault(region => region.Regionid == fmfr.PatientModel.State)?.Name;

            if (!userExists)
            {
                Aspnetuser user = new Aspnetuser();
                string id = Guid.NewGuid().ToString();
                user.Id = id;
                user.Email = fmfr.PatientModel.Email;
                user.Phonenumber = fmfr.PatientModel.PhoneNo;
                user.Username = fmfr.PatientModel.Email;
                user.Createddate = DateTime.Now;
                user.Phonenumber = PatientPhoneNumber;
                user.Role = "Patient";
                _context.Aspnetusers.Add(user);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = user.Id;
                user_obj.Firstname = fmfr.PatientModel.FirstName;
                user_obj.Lastname = fmfr.PatientModel.LastName;
                user_obj.Email = fmfr.PatientModel.Email;
                user_obj.Mobile = fmfr.PatientModel.PhoneNo;
                user_obj.Street = fmfr.PatientModel.Street;
                user_obj.City = fmfr.PatientModel.City;
                user_obj.State = StateName;
                user_obj.Zipcode = fmfr.PatientModel.ZipCode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                user_obj.Regionid = fmfr.PatientModel.State;
                user_obj.Mobile = PatientPhoneNumber;
                user_obj.Intyear = fmfr.PatientModel.DateOfBirth?.Year;
                user_obj.Intdate = fmfr.PatientModel.DateOfBirth?.Day;
                user_obj.Strmonth = fmfr.PatientModel.DateOfBirth?.Month.ToString();
                user_obj.Street = fmfr.PatientModel.Street;

                _context.Users.Add(user_obj);
                _context.SaveChanges();

                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 3;
                request.Userid = user_obj.Userid;  //
                request.Firstname = fmfr.FirstName;
                request.Lastname = fmfr.LastName;
                request.Phonenumber = FriendFamilyPhoneNo;
                request.Email = fmfr.Email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = id;  //
                request.Status = 1;
                request.Createduserid = user_obj.Userid;  //
                request.Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(user_obj);
                request.Phonenumber = FriendFamilyPhoneNo;

                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = fmfr.PatientModel.FirstName;
                rc.Lastname = fmfr.PatientModel.LastName;
                rc.Phonenumber = fmfr.PatientModel.PhoneNo;
                rc.Location = fmfr.PatientModel.City + fmfr.PatientModel.State;
                rc.Email = fmfr.PatientModel.Email;
                rc.Address = fmfr.PatientModel.RoomSuite + ", " + fmfr.PatientModel.Street + ", " + fmfr.PatientModel.City + ", " + fmfr.PatientModel.State + ", " + fmfr.PatientModel.ZipCode;
                rc.Street = fmfr.PatientModel.Street;
                rc.City = fmfr.PatientModel.City;
                rc.State = StateName;
                rc.Zipcode = fmfr.PatientModel.ZipCode;
                rc.Notes = fmfr.PatientModel.Symptoms;
                rc.Regionid = fmfr.PatientModel.State;
                rc.Intdate = fmfr.PatientModel.DateOfBirth?.Day;
                rc.Strmonth = fmfr.PatientModel.DateOfBirth?.Month.ToString();
                rc.Intyear = fmfr.PatientModel.DateOfBirth?.Year;
                rc.Phonenumber = PatientPhoneNumber;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                _emailService.SendEmailForPasswordSetup(rc.Firstname, rc.Lastname ?? "", rc.Email, SetUpLink);

                if (fmfr.PatientModel.File != null)
                {
                    _insertfiles.insertfilesunique(fmfr.PatientModel.File, uniqueid, path);
                    var filestring = Path.GetFileNameWithoutExtension(fmfr.PatientModel.File.FileName);
                    var extensionstring = Path.GetExtension(fmfr.PatientModel.File.FileName);
                    Requestwisefile rwf = new()
                    {
                        Requestid = request.Requestid,
                        Filename = uniqueid + "$" + fmfr.PatientModel.File.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(rwf);
                    _context.SaveChanges();
                }
            }
            else
            {
                User userdata = _context.Users.FirstOrDefault(user => user.Email == fmfr.PatientModel.Email);
                if (userdata != null)
                {
                    Request request = new Request();
                    //change the fname, lname , and contact detials acc to the requestor
                    request.Requesttypeid = 3;
                    request.Userid = userdata.Userid;
                    request.Firstname = fmfr.FirstName;
                    request.Lastname = fmfr.LastName;
                    request.Phonenumber = FriendFamilyPhoneNo;
                    request.Email = fmfr.Email;
                    request.Createddate = DateTime.Now;
                    request.Patientaccountid = userdata.Aspnetuserid;
                    request.Status = 1;
                    request.Createduserid = userdata.Userid;
                    request.Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(userdata);

                    _context.Requests.Add(request);
                    _context.SaveChanges();

                    Requestclient rc = new Requestclient();
                    rc.Requestid = request.Requestid;
                    rc.Firstname = fmfr.PatientModel.FirstName;
                    rc.Lastname = fmfr.PatientModel.LastName;
                    rc.Phonenumber = fmfr.PatientModel.PhoneNo;
                    rc.Location = fmfr.PatientModel.City + fmfr.PatientModel.State;
                    rc.Email = fmfr.PatientModel.Email;
                    rc.Address = fmfr.PatientModel.RoomSuite + ", " + fmfr.PatientModel.Street + ", " + fmfr.PatientModel.City + ", " + fmfr.PatientModel.State + ", " + fmfr.PatientModel.ZipCode;
                    rc.Street = fmfr.PatientModel.Street;
                    rc.City = fmfr.PatientModel.City;
                    rc.State = StateName;
                    rc.Zipcode = fmfr.PatientModel.ZipCode;
                    rc.Notes = fmfr.PatientModel.Symptoms;
                    rc.Regionid = fmfr.PatientModel.State;
                    rc.Intdate = fmfr.PatientModel.DateOfBirth?.Day;
                    rc.Strmonth = fmfr.PatientModel.DateOfBirth?.Month.ToString();
                    rc.Intyear = fmfr.PatientModel.DateOfBirth?.Year;
                    rc.Phonenumber = PatientPhoneNumber;

                    _context.Requestclients.Add(rc);
                    _context.SaveChanges();

                    if (fmfr.PatientModel.File != null)
                    {
                        _insertfiles.insertfilesunique(fmfr.PatientModel.File, uniqueid, path);
                        var filestring = Path.GetFileNameWithoutExtension(fmfr.PatientModel.File.FileName);
                        var extensionstring = Path.GetExtension(fmfr.PatientModel.File.FileName);
                        Requestwisefile rwf = new()
                        {
                            Requestid = request.Requestid,
                            Filename = uniqueid + "$" + fmfr.PatientModel.File.FileName,
                            Createddate = DateTime.Now,
                        };
                        _context.Requestwisefiles.Add(rwf);
                        _context.SaveChanges();
                    }
                }
            }
        }
        public void CRequest(ConciergeModel cm, string SetUpLink)
        {
            bool userExists = _context.Users.Any(PatientData => PatientData.Email == cm.PatientEmail);
            string PatientPhoneNumber = "+" + cm.PatientCountryCode + "-" + cm.PatientPhoneNo;
            string ConciergePhoneNo = "+" + cm.ConciergeCountryCode + "-" + cm.ConciergePhoneNo;
            var StateName = _context.Regions.FirstOrDefault(region => region.Regionid == cm.ConciergeState)?.Name;

            if (!userExists)
            {
                Aspnetuser user = new Aspnetuser();

                string id = Guid.NewGuid().ToString();
                user.Id = id;
                user.Email = cm.PatientEmail;
                user.Phonenumber = cm.PatientPhoneNo;
                user.Username = cm.PatientEmail;
                user.Createddate = DateTime.Now;
                user.Phonenumber = PatientPhoneNumber;
                user.Role = "Patient";
                _context.Aspnetusers.Add(user);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = user.Id;
                user_obj.Firstname = cm.PatientFirstName;
                user_obj.Lastname = cm.PatientLastName;
                user_obj.Email = cm.PatientEmail;
                user_obj.Mobile = cm.PatientPhoneNo;
                user_obj.Street = cm.ConciergeStreet;
                user_obj.City = cm.ConciergeCity;
                user_obj.State = StateName;
                user_obj.Zipcode = cm.ConciergeZipCode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                user_obj.Regionid = cm.ConciergeState;
                user_obj.Mobile = PatientPhoneNumber;
                user_obj.Intyear = cm.PatientDateOfBirth?.Year;
                user_obj.Intdate = cm.PatientDateOfBirth?.Day;
                user_obj.Strmonth = cm.PatientDateOfBirth?.Month.ToString();
                _context.Users.Add(user_obj);
                _context.SaveChanges();

                Concierge conciergeData = new()
                {
                    Conciergename = cm.ConciergeFirstName + cm.ConciergeLastName,
                    Street = cm.ConciergeStreet,
                    City = cm.ConciergeCity,
                    State = StateName,
                    Zipcode = cm.ConciergeZipCode,
                    Createddate = DateTime.Now,
                    Address = cm.ConciergeStreet + " " + cm.ConciergeCity + " " + cm.ConciergeState + " " + cm.ConciergeZipCode,
                    Regionid = cm.ConciergeState
                };
                _context.Concierges.Add(conciergeData);
                _context.SaveChanges();

                Request req = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                req.Requesttypeid = 4;
                req.Userid = user_obj.Userid;
                req.Firstname = cm.ConciergeFirstName;
                req.Lastname = cm.ConciergeLastName;
                req.Email = cm.ConciergeEmail;
                req.Createddate = DateTime.Now;
                req.Patientaccountid = id;
                req.Status = 1;
                req.Createduserid = user_obj.Userid;
                req.Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(user_obj);
                req.Phonenumber = ConciergePhoneNo;

                _context.Requests.Add(req);
                _context.SaveChanges();

                Requestconcierge rec = new()
                {
                    Requestid = req.Requestid,
                    Conciergeid = conciergeData.Conciergeid
                };
                _context.Requestconcierges.Add(rec);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = req.Requestid;
                rc.Firstname = cm.PatientFirstName;
                rc.Lastname = cm.PatientLastName;
                rc.Phonenumber = cm.PatientPhoneNo;
                rc.Location = cm.ConciergeCity + StateName;
                rc.Email = cm.PatientEmail;
                rc.Address = cm.PatientRoomNo + ", " + cm.ConciergeStreet + ", " + cm.ConciergeCity + ", " + cm.ConciergeState + ", " + cm.ConciergeZipCode;
                rc.Street = cm.ConciergeStreet;
                rc.City = cm.ConciergeCity;
                rc.State = StateName;
                rc.Zipcode = cm.ConciergeZipCode;
                rc.Notes = cm.PatientSymptoms;
                rc.Regionid = cm.ConciergeState;
                rc.Intdate = cm.PatientDateOfBirth?.Day;
                rc.Strmonth = cm.PatientDateOfBirth?.Month.ToString();
                rc.Intyear = cm.PatientDateOfBirth?.Year;
                rc.Phonenumber = PatientPhoneNumber;
                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                _emailService.SendEmailForPasswordSetup(rc.Firstname, rc.Lastname ?? "", rc.Email, SetUpLink);

            }
            else
            {
                User? userdata = _context.Users.FirstOrDefault(user => user.Email == cm.PatientEmail);
                if (userdata != null)
                {
                    Concierge conciergeData = new()
                    {
                        Conciergename = cm.ConciergeFirstName + cm.ConciergeLastName,
                        Street = cm.ConciergeStreet,
                        City = cm.ConciergeCity,
                        State = StateName,
                        Zipcode = cm.ConciergeZipCode,
                        Createddate = DateTime.Now,
                        Address = cm.ConciergeStreet + " " + cm.ConciergeCity + " " + cm.ConciergeState + " " + cm.ConciergeZipCode,
                        Regionid = cm.ConciergeState
                    };
                    _context.Concierges.Add(conciergeData);
                    _context.SaveChanges();

                    Request req = new Request();
                    //change the fname, lname , and contact detials acc to the requestor
                    req.Requesttypeid = 4;
                    req.Firstname = cm.ConciergeFirstName;
                    req.Lastname = cm.ConciergeLastName;
                    req.Email = cm.ConciergeEmail;
                    req.Createddate = DateTime.Now;
                    req.Status = 1;
                    req.Phonenumber = ConciergePhoneNo;
                    req.Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(userdata);
                    req.Userid = userdata.Userid;
                    _context.Requests.Add(req);
                    _context.SaveChanges();

                    Requestconcierge rec = new()
                    {
                        Requestid = req.Requestid,
                        Conciergeid = conciergeData.Conciergeid
                    };
                    _context.Requestconcierges.Add(rec);
                    _context.SaveChanges();

                    Requestclient rc = new Requestclient();
                    rc.Requestid = req.Requestid;
                    rc.Firstname = cm.PatientFirstName;
                    rc.Lastname = cm.PatientLastName;
                    rc.Phonenumber = cm.PatientPhoneNo;
                    rc.Location = cm.ConciergeCity + StateName;
                    rc.Email = cm.PatientEmail;
                    rc.Address = cm.PatientRoomNo + ", " + cm.ConciergeStreet + ", " + cm.ConciergeCity + ", " + cm.ConciergeState + ", " + cm.ConciergeZipCode;
                    rc.Street = cm.ConciergeStreet;
                    rc.City = cm.ConciergeCity;
                    rc.State = StateName;
                    rc.Zipcode = cm.ConciergeZipCode;
                    rc.Notes = cm.PatientSymptoms;
                    rc.Regionid = cm.ConciergeState;
                    rc.Intdate = cm.PatientDateOfBirth?.Day;
                    rc.Strmonth = cm.PatientDateOfBirth?.Month.ToString();
                    rc.Intyear = cm.PatientDateOfBirth?.Year;
                    rc.Phonenumber = PatientPhoneNumber;
                    _context.Requestclients.Add(rc);
                    _context.SaveChanges();
                }
            }
        }
        public void PRequest(PatientModel pm, string uniqueid, string _path)
        {
            Region StateName = _context.Regions.FirstOrDefault(region => region.Regionid == pm.State);
            string PatientPhone = "+" + pm.CountryCode + "-" + pm.PhoneNo;
            if (pm.Password != null)
            {
                //var newvm=new PatientModel();
                Aspnetuser user = new Aspnetuser();

                string id = Guid.NewGuid().ToString();
                user.Id = id;
                user.Email = pm.Email;
                user.Passwordhash = _passwordHasher.GenerateSHA256(pm.Password);
                user.Phonenumber = pm.PhoneNo;
                user.Username = pm.FirstName;
                user.Createddate = DateTime.Now;
                user.Phonenumber = PatientPhone;
                user.Role = "Patient";
                _context.Aspnetusers.Add(user);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = user.Id;
                user_obj.Firstname = pm.FirstName;
                user_obj.Lastname = pm.LastName;
                user_obj.Email = pm.Email;
                user_obj.Mobile = pm.PhoneNo;
                user_obj.Street = pm.Street;
                user_obj.City = pm.City;
                user_obj.State = StateName.Name;
                user_obj.Zipcode = pm.ZipCode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                user_obj.Regionid = pm.State;
                user_obj.Mobile = PatientPhone;
                user_obj.Intyear = pm.DateOfBirth?.Year;
                user_obj.Intdate = pm.DateOfBirth?.Day;
                user_obj.Strmonth = pm.DateOfBirth?.Month.ToString();

                _context.Users.Add(user_obj);
                _context.SaveChanges();

                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Userid = user_obj.Userid;
                request.Firstname = pm.FirstName;
                request.Lastname = pm.LastName;
                request.Email = pm.Email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = id;
                request.Status = 1;
                request.Createduserid = user_obj.Userid;
                request.Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(user_obj);
                request.Phonenumber = PatientPhone;

                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = pm.FirstName;
                rc.Lastname = pm.LastName;
                rc.Phonenumber = pm.PhoneNo;
                rc.Location = pm.City + pm.State;
                rc.Email = pm.Email;
                rc.Address = pm.RoomSuite + ", " + pm.Street + ", " + pm.City + ", " + pm.State + ", " + pm.ZipCode;
                rc.Street = pm.Street;
                rc.City = pm.City;
                rc.State = StateName.Name;
                rc.Zipcode = pm.ZipCode;
                rc.Notes = pm.Symptoms;
                rc.Regionid = pm.State;
                rc.Intdate = pm.DateOfBirth?.Day;
                rc.Strmonth = pm.DateOfBirth?.Month.ToString();
                rc.Intyear = pm.DateOfBirth?.Year;
                rc.Phonenumber = PatientPhone;
                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                if (pm.File != null)
                {

                    _insertfiles.insertfilesunique(pm.File, uniqueid, _path);
                    var filestring = Path.GetFileNameWithoutExtension(pm.File.FileName);
                    var extensionstring = Path.GetExtension(pm.File.FileName);
                    Requestwisefile rwf = new()
                    {
                        Requestid = request.Requestid,
                        Filename = uniqueid + "$" + pm.File.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(rwf);
                    _context.SaveChanges();
                }
            }
            else
            {
                User user_obj = _context.Users.FirstOrDefault(u => u.Email == pm.Email);
                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Userid = user_obj.Userid;
                request.Firstname = pm.FirstName;
                request.Lastname = pm.LastName;
                request.Phonenumber = pm.PhoneNo;
                request.Email = pm.Email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = user_obj.Aspnetuserid;
                request.Status = 1;
                request.Createduserid = user_obj.Userid;
                request.Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(user_obj);
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = pm.FirstName;
                rc.Lastname = pm.LastName;
                rc.Phonenumber = pm.PhoneNo;
                rc.Location = pm.City + pm.State;
                rc.Email = pm.Email;
                rc.Address = pm.RoomSuite + ", " + pm.Street + ", " + pm.City + ", " + pm.State + ", " + pm.ZipCode;
                rc.Street = pm.Street;
                rc.City = pm.City;
                rc.State = StateName.Name;
                rc.Zipcode = pm.ZipCode;
                rc.Notes = pm.Symptoms;
                rc.Regionid = pm.State;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();
                if (pm.File != null)
                {

                    _insertfiles.insertfiles(pm.File, _path);
                    Requestwisefile rwf = new()
                    {
                        Requestid = request.Requestid,
                        Filename = pm.File.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(rwf);
                    _context.SaveChanges();
                }
            }
        }
        public void BRequest(BusinessModel bm , string SetUpLink)
        {
            bool userExists = _context.Users.Any(PatientData => PatientData.Email == bm.PatientEmail);
            string PatientPhoneNumber = "+" + bm.PatientCountryCode + "-" + bm.PatientPhoneNo;
            string BusinessPhoneNo = "+" + bm.BusinessCountryCode + "-" + bm.BusinessPhoneNo;
            var StateName = _context.Regions.FirstOrDefault(region => region.Regionid == bm.State)?.Name;

            if (!userExists)
            {
                Aspnetuser user = new Aspnetuser();

                string id = Guid.NewGuid().ToString();
                user.Id = id;
                user.Email = bm.PatientEmail;
                user.Phonenumber = bm.PatientPhoneNo;
                user.Username = bm.PatientEmail;
                user.Createddate = DateTime.Now;
                user.Phonenumber = BusinessPhoneNo;

                user.Role = "Patient";
                _context.Aspnetusers.Add(user);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = user.Id;
                user_obj.Firstname = bm.PatientFirstName;
                user_obj.Lastname = bm.PatientLastName;
                user_obj.Email = bm.PatientEmail;
                user_obj.Mobile = bm.PatientPhoneNo;
                user_obj.Street = bm.Street;
                user_obj.City = bm.City;
                user_obj.State = StateName;
                user_obj.Zipcode = bm.ZipCode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                user_obj.Regionid = bm.State;
                user_obj.Mobile = PatientPhoneNumber;
                user_obj.Intyear = bm.PatientDateOfBirth?.Year;
                user_obj.Intdate = bm.PatientDateOfBirth?.Day;
                user_obj.Strmonth = bm.PatientDateOfBirth?.Month.ToString();

                _context.Users.Add(user_obj);
                _context.SaveChanges();

                //!!!!!!!!!!!!!!!!!!!!Doubt!!!!!!!!!!!!!!!!!!!!
                //where to get data of address1, address 2 , region id and all other stuff
                Business business = new()
                {
                    Name = bm.BusinessName,
                    Phonenumber = BusinessPhoneNo,
                    Createddate = DateTime.Now,
                };
                _context.Businesses.Add(business);
                _context.SaveChanges();

                Request req = new()
                {
                    Requesttypeid = 1,
                    Firstname = bm.BusinessFirstName,
                    Lastname = bm.BusinessLastName,
                    Phonenumber = BusinessPhoneNo,
                    Email = bm.BusinessEmail,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Userid = user_obj.Userid,
                    Createduserid = user_obj.Userid,
                    Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(user_obj),
                    Patientaccountid = id
                };
                _context.Requests.Add(req);
                _context.SaveChanges();

                Requestbusiness ReqBus = new()
                {
                    Requestid = req.Requestid,
                    Businessid = business.Id,
                };
                _context.Requestbusinesses.Add(ReqBus);
                _context.SaveChanges();

                Requestclient rc = new()
                {
                    Requestid = req.Requestid,
                    Firstname = bm.PatientFirstName,
                    Lastname = bm.BusinessLastName,
                    Phonenumber = PatientPhoneNumber,
                    Street = bm.Street,
                    City = bm.City,
                    State = StateName,
                    Zipcode = bm.ZipCode,
                    Intdate = bm.PatientDateOfBirth?.Day,
                    Intyear = bm.PatientDateOfBirth?.Year,
                    Strmonth = bm.PatientDateOfBirth?.Month.ToString(),
                    Email=bm.PatientEmail
                };
                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                _emailService.SendEmailForPasswordSetup(rc.Firstname, rc.Lastname, rc.Email, SetUpLink);

            }
            else
            {
                User userData = _context.Users.FirstOrDefault(user=>user.Email==bm.PatientEmail);
                if (userData != null)
                {
                    //!!!!!!!!!!!!!!!!!!!!Doubt!!!!!!!!!!!!!!!!!!!!
                    //where to get data of address1, address 2 , region id and all other stuff
                    Business business = new()
                    {
                        Name = bm.BusinessName,
                        Phonenumber = BusinessPhoneNo,
                        Createddate = DateTime.Now,
                    };
                    _context.Businesses.Add(business);
                    _context.SaveChanges();

                    Request req = new()
                    {
                        Requesttypeid = 1,
                        Firstname = bm.BusinessFirstName,
                        Lastname = bm.BusinessLastName,
                        Phonenumber = BusinessPhoneNo,
                        Email = bm.BusinessEmail,
                        Status = 1,
                        Createddate = DateTime.Now,
                        Userid = userData.Userid,
                        Createduserid = userData.Userid,
                        Confirmationnumber = _passwordHasher.GenerateConfirmationNumber(userData),
                        Patientaccountid = userData.Aspnetuserid
                    };
                    _context.Requests.Add(req);
                    _context.SaveChanges();

                    Requestbusiness ReqBus = new()
                    {
                        Requestid = req.Requestid,
                        Businessid = business.Id,
                    };
                    _context.Requestbusinesses.Add(ReqBus);
                    _context.SaveChanges();

                    Requestclient rc = new()
                    {
                        Requestid = req.Requestid,
                        Firstname = bm.PatientFirstName,
                        Lastname = bm.BusinessLastName,
                        Phonenumber = PatientPhoneNumber,
                        Street = bm.Street,
                        City = bm.City,
                        State = StateName,
                        Zipcode = bm.ZipCode,
                        Intdate = bm.PatientDateOfBirth?.Day,
                        Intyear = bm.PatientDateOfBirth?.Year,
                        Strmonth = bm.PatientDateOfBirth?.Month.ToString()
                    };
                    _context.Requestclients.Add(rc);
                    _context.SaveChanges();
                }
            }
        }
    }
}