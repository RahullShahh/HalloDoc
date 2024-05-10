using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interfaces;
using BAL.Interfaces.IProvider;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using DocumentFormat.OpenXml.Spreadsheet;
using HalloDoc_Project.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using static HalloDoc_Project.Extensions.Enumerations;


namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Physician")]

    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminActions _adminActions;
        private readonly IAdminTables _adminTables;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileOperations _fileOperations;
        private readonly INotyfService _notyf;
        private readonly IEncounterForm _encounterForm;
        private readonly ICreateEditProviderRepo _createEditProviderRepo;
        private readonly IPasswordHasher _passwordHasher;
        public ProviderController(ApplicationDbContext context, IAdminActions adminActions, IAdminTables adminTables, IEmailService emailService, IWebHostEnvironment environment, IFileOperations fileOperations, INotyfService notyf, IEncounterForm encounterForm, ICreateEditProviderRepo createEditProviderRepo, IPasswordHasher passwordHasher)
        {
            _context = context;
            _adminActions = adminActions;
            _adminTables = adminTables;
            _emailService = emailService;
            _environment = environment;
            _fileOperations = fileOperations;
            _notyf = notyf;
            _encounterForm = encounterForm;
            _createEditProviderRepo = createEditProviderRepo;
            _passwordHasher = passwordHasher;
        }

        public IActionResult ProviderDashboard()
        {
            try
            {
                var id = HttpContext.Session.GetString("AspnetuserId");
                AdminDashboardViewModel model = _adminTables.ProviderDashboard(id);
                return View(model);
            }
            catch
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("jwt");
                _notyf.Error("Exception in Provider Dashboard.");
                return RedirectToAction("login_page", "Guest");
            }
        }
        public DashboardFilter SetDashboardFilterValues(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            return filter;
        }
        public IActionResult GetNewTable(int page, int region, int type, string search)
        {
            try
            {

                var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
                Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.ProviderNewTable(filter, physician.Physicianid);
                model.currentPage = filter.pageNumber;

                return View("PartialTables/ProviderNewTable", model);
            }
            catch
            {

                _notyf.Error("Exception in fetching new table get.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult GetPendingTable(int page, int region, int type, string search)
        {
            try
            {
                var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
                Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.ProviderPendingTable(filter, physician.Physicianid);
                model.currentPage = filter.pageNumber;

                return PartialView("PartialTables/ProviderPendingTable", model);
            }
            catch
            {

                _notyf.Error("Exception in fetching pending table get.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult GetActiveTable(int page, int region, int type, string search)
        {
            try
            {

                var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
                Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.ProviderActiveTable(filter, physician.Physicianid);
                model.currentPage = filter.pageNumber;

                return PartialView("PartialTables/ProviderActiveTable", model);
            }
            catch
            {
                _notyf.Error("Exception in fetching active table get.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult GetConcludeTable(int page, int region, int type, string search)
        {
            try
            {
                var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
                Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.ProviderConcludeTable(filter, physician.Physicianid);
                model.currentPage = filter.pageNumber;

                return PartialView("PartialTables/ProviderConcludeTable", model);
            }

            catch
            {
                _notyf.Error("Exception in fetching coclude table get.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult AcceptCase(int requestid)
        {
            try
            {
                _adminActions.ProviderAcceptCase(requestid);
                return RedirectToAction("ProviderDashboard");
            }
            catch
            {
                _notyf.Error("Exception in accepting the case.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult ProviderViewCase(int requestid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewCaseViewModel vc = _adminActions.ViewCaseAction(requestid);
                    return View("ActionViews/ProviderViewCase", vc);
                }
                return View("ActionViews/ProviderViewCase");
            }
            catch
            {
                _notyf.Error("Exception in Provider View Case.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult ProviderViewNotes()
        {
            return View("ActionViews/ProviderViewNotes");
        }
        [HttpPost]
        public IActionResult SendAgreement(int RequestId, string PhoneNo, string email)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var AgreementLink = Url.Action("ReviewAgreement", "Guest", new { ReqId = RequestId }, Request.Scheme);
                    _emailService.SendAgreementLink(RequestId, AgreementLink, email);
                    _notyf.Success("Agreement link sent to user.");
                    return RedirectToAction("AdminDashboard", "Guest");
                }
                return View();
            }
            catch
            {
                _notyf.Error("Exception in Sending agreement.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        #region Send Link
        [HttpPost]
        public void SendLink(string FirstName, string LastName, string Email)
        {
            try
            {
                var WebsiteLink = Url.Action("patient_submit_request_screen", "Guest", new { }, Request.Scheme);
                _emailService.SendEmailWithLink(FirstName, LastName, Email, WebsiteLink);
                _notyf.Success("Link sent.");
            }
            catch
            {
                _notyf.Error("Exception in accepting the case.");

            }
        }
        #endregion

        #region Create Request

        public IActionResult CreateRequestProviderDashboard()
        {
            try
            {

                CreateRequestViewModel model = new CreateRequestViewModel()
                {
                    Regions = _context.Regions.ToList(),
                };
                return View(model);
            }
            catch
            {
                _notyf.Error("Exception in Create request provider dashboard.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        [HttpPost]
        [RoleAuthorize((int)AllowMenu.AdminDashboard)]
        public IActionResult CreateRequestProviderDashboard(CreateRequestViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _adminActions.CreateRequestFromAdminDashboard(model);
                }
                return RedirectToAction("CreateRequestProviderDashboard");
            }
            catch
            {
                _notyf.Error("Exception in Create request provider dashboard.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        #endregion

        #region Transfer Case Methods

        [HttpPost]
        public IActionResult TransferCase(int RequestId, string TransferPhysician, string TransferDescription)
        {
            try
            {
                var phyId = HttpContext.Session.GetString("AspnetuserId");
                var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == phyId);
                _adminActions.ProviderTransferCase(RequestId, TransferPhysician, TransferDescription, physician.Physicianid);
                _notyf.Success("Case transferred successfully");
                return Ok();
            }
            catch
            {
                _notyf.Error("Exception in transfer case");
                return RedirectToAction("ProviderDashboard");
            }

        }
        #endregion

        #region Send Orders Methods
        public List<Healthprofessional>? filterVenByPro(string ProfessionId)
        {
            try
            {
                var result = _context.Healthprofessionals.Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
                return result;
            }
            catch
            {
                _notyf.Error("Exception in filtering vendor by profession");
                return null;
            }
        }
        public IActionResult BusinessData(int BusinessId)
        {
            try
            {

                var result = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == BusinessId);
                return Json(result);
            }
            catch
            {
                _notyf.Error("Exception in Business Data.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult SendOrders(int requestid)
        {
            try
            {

                List<Healthprofessional> healthprofessionals = _context.Healthprofessionals.ToList();
                List<Healthprofessionaltype> healthprofessionaltypes = _context.Healthprofessionaltypes.ToList();
                SendOrderViewModel model = new()
                {
                    requestid = requestid,
                    healthprofessionals = healthprofessionals,
                    healthprofessionaltype = healthprofessionaltypes
                };
                return View("ActionViews/ProviderSendOrders", model);
            }
            catch
            {
                _notyf.Error("Exception in Send Orders Get");
                return RedirectToAction("ProviderDashboard");
            }
        }

        [HttpPost]
        public IActionResult SendOrders(int requestid, SendOrderViewModel sendOrder)
        {
            try
            {

                _adminActions.SendOrderAction(requestid, sendOrder);
                return RedirectToAction("ProviderDashboard");
            }
            catch
            {
                _notyf.Error("Exception in Send Orders post");
                return RedirectToAction("ProviderDashboard");
            }
        }
        #endregion

        #region LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("login_page", "Guest");
        }
        #endregion

        #region CONCLUDE CARE
        public IActionResult ConcludeCareDeleteFile(int fileid, int requestid)
        {
            try
            {

                var fileRequest = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
                if (fileRequest != null)
                {
                    fileRequest.Isdeleted = true;
                    _context.Requestwisefiles.Update(fileRequest);
                }
                _context.SaveChanges();
                return RedirectToAction("ProviderConcludeCare", new { requestid = requestid });
            }
            catch
            {
                _notyf.Error("Exception in Conclude Care delete file");
                return RedirectToAction("ProviderDashboard");
            }
        }
        [HttpPost]
        public IActionResult ProviderConcludeCare(ViewUploadsViewModel uploads)
        {
            try
            {
                if (uploads.File != null)
                {
                    var uniqueid = Guid.NewGuid().ToString();
                    var path = _environment.WebRootPath;
                    _fileOperations.insertfilesunique(uploads.File, uniqueid, path);

                    var filestring = Path.GetFileNameWithoutExtension(uploads.File.FileName);
                    var extensionstring = Path.GetExtension(uploads.File.FileName);
                    Requestwisefile requestwisefile = new()
                    {
                        Filename = uniqueid + "$" + uploads.File.FileName,
                        Requestid = uploads.RequestID,
                        Createddate = DateTime.Now
                    };
                    _context.Update(requestwisefile);
                    _context.SaveChanges();
                }
                return RedirectToAction("ProviderConcludeCare", new { requestid = uploads.RequestID });
            }
            catch
            {
                _notyf.Error("Exception in Provider Conclude Care post");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult ProviderConcludeCare(int requestid)
        {
            try
            {
                var getPhysician = HttpContext.Session.GetString("AspnetuserId");
                var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);

                var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
                var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
                var patients = _context.Requestclients.FirstOrDefault(r => r.Requestid == requestid);
                var encounterform = _context.Encounterforms.FirstOrDefault(r => r.Requestid == requestid);

                ViewUploadsViewModel uploads = new()
                {
                    ConfirmationNo = user.Confirmationnumber ?? "",
                    Patientname = patients.Firstname + " " + patients.Lastname ?? "",
                    RequestID = requestid,
                    Requestwisefiles = requestFile,
                    PhysicianId = Physicianid.Physicianid
                };
                if (encounterform != null)
                {
                    uploads.isFinalized = encounterform.Isfinalize;
                }
                else
                {
                    uploads.isFinalized = false;
                }

                return View("ActionViews/ProviderConcludeCare", uploads);
            }
            catch
            {
                _notyf.Error("Exception in Provider Conclude Care ");
                return RedirectToAction("ProviderDashboard");
            }
        }
        [HttpPost]
        public IActionResult ConcludeCare(ViewUploadsViewModel model)
        {
            try
            {

                var request = _context.Requests.FirstOrDefault(req => req.Requestid == model.RequestID);

                if (request != null)
                {
                    request.Status = (int)RequestStatus.Closed;
                }
                _context.Requests.Update(request);
                Requeststatuslog statusLog = new()
                {
                    Requestid = model.RequestID,
                    Status = (int)RequestStatus.Closed,
                    Physicianid = model.PhysicianId,
                    Notes = model.ProviderNotes,
                    Createddate = DateTime.Now
                };
                _context.SaveChanges();
                return RedirectToAction("ProviderDashboard");
            }
            catch
            {
                _notyf.Error("Exception in Conclude Care");
                return RedirectToAction("ProviderDashboard");
            }
        }

        #endregion

        #region MY PROFILE
        public IActionResult ProviderMyProfile()
        {
            try
            {

                var physician = HttpContext.Session.GetString("AspnetuserId");
                var getPhysician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == physician);
                EditPhysicianViewModel EditPhysician = _createEditProviderRepo.ProviderDashboardGetPhysicianDetailsForEditPro(getPhysician.Physicianid);
                return View("ProviderMyProfile", EditPhysician);
            }
            catch
            {
                _notyf.Error("Exception in Provider My Profile");
                return RedirectToAction("ProviderDashboard");
            }
        }

        [HttpPost]
        public IActionResult SubmitPhysicianAccountInfo(EditPhysicianViewModel PhysicianAccountInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == PhysicianAccountInfo.PhysicianId);
                    var PhysicianUserDetails = _context.Aspnetusers.FirstOrDefault(x => x.Id == Physician.Aspnetuserid);
                    if (PhysicianUserDetails != null)
                    {
                        Aspnetuser PhysicianUser = new()
                        {
                            Passwordhash = _passwordHasher.GenerateSHA256(PhysicianUserDetails.Passwordhash)
                        };
                        _context.Aspnetusers.Update(PhysicianUserDetails);
                        _context.SaveChanges();
                    }
                }
                _notyf.Success("Details Updated Successfully.");
                return ProviderMyProfile();
            }
            catch
            {
                _notyf.Error("Exception in submit physician account info post");
                return RedirectToAction("ProviderDashboard");
            }
        }
        #endregion

        #region View Uploads 
        [HttpPost]
        public IActionResult ViewUploads(ViewUploadsViewModel uploads)
        {
            try
            {
                if (uploads.File != null)
                {
                    var uniqueid = Guid.NewGuid().ToString();
                    var path = _environment.WebRootPath;
                    _fileOperations.insertfilesunique(uploads.File, uniqueid, path);

                    var filestring = Path.GetFileNameWithoutExtension(uploads.File.FileName);
                    var extensionstring = Path.GetExtension(uploads.File.FileName);
                    Requestwisefile requestwisefile = new()
                    {
                        Filename = uniqueid + "$" + uploads.File.FileName,
                        Requestid = uploads.RequestID,
                        Createddate = DateTime.Now
                    };
                    _context.Update(requestwisefile);
                    _context.SaveChanges();
                    _notyf.Success("Document Uploaded.");
                }
                return RedirectToAction("ViewUploads", new { requestid = uploads.RequestID });
            }
            catch
            {
                _notyf.Error("Exception in View Uploads");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult DeleteFile(int fileid, int requestid)
        {
            try
            {

                var fileRequest = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
                fileRequest.Isdeleted = true;

                _context.Requestwisefiles.Update(fileRequest);
                _context.SaveChanges();

                return RedirectToAction("ViewUploads", new { requestid = requestid });
            }
            catch
            {
                _notyf.Error("Exception in Delete File Function.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult DeleteAllFiles(int requestid)
        {
            try
            {

                var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
                for (int i = 0; i < request.Count; i++)
                {
                    request[i].Isdeleted = true;
                    _context.Update(request[i]);
                }
                _context.SaveChanges();
                return RedirectToAction("ViewUploads", new { requestid = requestid });
            }
            catch
            {
                _notyf.Error("Exception in Delete all files function.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult ViewUploads(int requestid)
        {
            try
            {
                var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
                var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
                var requests = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

                ViewUploadsViewModel uploads = new()
                {
                    ConfirmationNo = requests.Confirmationnumber,
                    Patientname = user.Firstname + " " + user.Lastname,
                    RequestID = requestid,
                    Requestwisefiles = requestFile
                };
                return View("ActionViews/ProviderViewUploads", uploads);
            }

            catch
            {
                _notyf.Error("Exception in View Uploads Get Method.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult SendMail(int requestid)
        {
            try
            {
                var path = _environment.WebRootPath;
                //_emailService.SendEmailWithAttachments(requestid, path);
                return RedirectToAction("ViewUploads", "Provider", new { requestid = requestid });
            }
            catch
            {
                _notyf.Error("Exception in Send Mail.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        #endregion

        #region Encounter Form Methods

        //encounter form call type modal methods

        [HttpPost]
        public bool EncounterHouseCallBegin(int requestId)
        {

            var getPhysician = HttpContext.Session.GetString("AspnetuserId");
            var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
            try
            {
                Request? request = _context.Requests.FirstOrDefault(req => req.Requestid == requestId);
                if (request == null)
                {
                    _notyf.Error("Cannot find request. Please try again later.");
                    return false;
                }

                DateTime currentTime = DateTime.Now;

                request.Status = (int)RequestStatus.MDOnSite;
                request.Modifieddate = currentTime;
                request.Calltype = (int)RequestCallType.HouseCall;


                _context.Requests.Update(request);

                string logNotes = Physicianid.Firstname + " " + Physicianid.Lastname + " started house call encounter on " + currentTime.ToString("MM/dd/yyyy") + " at " + currentTime.ToString("HH:mm:ss");

                Requeststatuslog reqStatusLog = new Requeststatuslog()
                {
                    Requestid = requestId,
                    Status = (short)RequestStatus.MDOnSite,
                    Physicianid = Physicianid.Physicianid,
                    Notes = logNotes,
                    Createddate = currentTime,
                };

                _context.Requeststatuslogs.Add(reqStatusLog);
                _context.SaveChanges();
                _notyf.Success("Successfully Started House Call Consultation.");
                return true;
            }
            catch
            {
                _notyf.Error("Exception in beginning encounter house call.");
                return false;
            }
        }

        public IActionResult EncounterHouseCallFinish(int requestId)
        {
            var getPhysician = HttpContext.Session.GetString("AspnetuserId");
            var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
            try
            {
                Request? request = _context.Requests.FirstOrDefault(req => req.Requestid == requestId);
                if (request == null)
                {
                    _notyf.Error("Cannot find request. Please try again later.");
                    return RedirectToAction("ProviderDashboard");
                }

                DateTime currentTime = DateTime.Now;

                request.Status = (int)RequestStatus.Conclude;
                request.Modifieddate = currentTime;
                request.Calltype = (int)RequestCallType.HouseCall;

                _context.Requests.Update(request);

                string logNotes = Physicianid.Firstname + " " + Physicianid.Lastname + " finished house call encounter on " + currentTime.ToString("MM/dd/yyyy") + " at " + currentTime.ToString("HH:mm:ss");

                Requeststatuslog reqStatusLog = new()
                {
                    Requestid = requestId,
                    Status = (short)RequestStatus.Conclude,
                    Physicianid = Physicianid.Physicianid,
                    Notes = logNotes,
                    Createddate = currentTime,
                };

                _context.Requeststatuslogs.Add(reqStatusLog);

                _context.SaveChanges();

                _notyf.Success("Successfully ended House Call Consultation.");

                return RedirectToAction("ProviderDashboard");
            }
            catch
            {
                _notyf.Error("Exception in finishing house call.");
                return RedirectToAction("ProviderDashboard");
            }
        }

        [HttpPost]
        public bool EncounterConsult(int requestId)
        {

            var getPhysician = HttpContext.Session.GetString("AspnetuserId");
            var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
            try
            {
                Request? request = _context.Requests.FirstOrDefault(req => req.Requestid == requestId);
                if (request == null)
                {
                    _notyf.Error("Cannot find request. Please try again later.");
                    return false;
                }

                DateTime currentTime = DateTime.Now;

                request.Status = (int)RequestStatus.Conclude;
                request.Modifieddate = currentTime;
                request.Calltype = (int)RequestCallType.HouseCall;

                _context.Requests.Update(request);

                string logNotes = Physicianid.Firstname + " " + Physicianid.Lastname + " consulted the request on " + currentTime.ToString("MM/dd/yyyy") + " at " + currentTime.ToString("HH:mm:ss");

                Requeststatuslog reqStatusLog = new Requeststatuslog()
                {
                    Requestid = requestId,
                    Status = (short)RequestStatus.Conclude,
                    Physicianid = Physicianid.Physicianid,
                    Notes = logNotes,
                    Createddate = currentTime,
                };
                _context.Requeststatuslogs.Add(reqStatusLog);
                _context.SaveChanges();
                _notyf.Success("Successfully Consulted Request.");
                return true;
            }
            catch
            {
                _notyf.Error("Exception in beginning encounter consulting.");
                return false;
            }
        }

        [HttpGet]
        public IActionResult FinalizeDownload(int requestid)
        {
            try
            {
                var EncounterModel = _encounterForm.EncounterFormGet(requestid);
                if (EncounterModel == null)
                {
                    return NotFound();
                }
                return new ViewAsPdf("ActionViews/EncounterFormFinalizeView", EncounterModel)
                {
                    FileName = "FinalizedEncounterForm.pdf"
                };
            }
            catch
            {
                _notyf.Error("Exception in Finalizing Download.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult FinalizeForm(int requestid)
        {
            try
            {
                Encounterform encounterRecord = _context.Encounterforms.FirstOrDefault(x => x.Requestid == requestid);
                if (encounterRecord != null)
                {
                    encounterRecord.Isfinalize = true;
                    _context.Encounterforms.Update(encounterRecord);
                }
                _context.SaveChanges();
                _notyf.Success("Encounter form finalized.");
                return RedirectToAction("ProviderDashboard", "Provider");
            }
            catch
            {
                _notyf.Error("Exception in finalizing the form.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        public IActionResult EncounterForm(int requestId, EncounterFormViewModel EncModel)
        {
            try
            {
                EncModel = _encounterForm.EncounterFormGet(requestId);
                var RequestExistStatus = _context.Encounterforms.FirstOrDefault(x => x.Requestid == requestId);
                if (RequestExistStatus == null)
                {
                    EncModel.IfExists = false;
                }
                if (RequestExistStatus != null)
                {
                    EncModel.IfExists = true;
                }
                return View("ActionViews/ProviderEncounterForm", EncModel);
            }
            catch
            {
                _notyf.Error("Exception in opening encounter form.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        [HttpPost]
        public IActionResult EncounterForm(EncounterFormViewModel model)
        {
            try
            {

                _encounterForm.EncounterFormPost(model.requestId, model);
                return EncounterForm(model.requestId, model);
            }
            catch
            {
                _notyf.Error("Exception in Encounter Form Post method.");
                return RedirectToAction("ProviderDashboard");
            }
        }

        //set consultancy type
        //public IActionResult SetConsultancyType(int typeId, int requestId)
        //{
        //    Request request = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);
        //    if (request != null)
        //    {
        //        //for housecall
        //        if (typeId == 1)
        //        {
        //            request.Calltype = 1;
        //            request.Status = 5;

        //            _context.Requests.Update(request);
        //        }
        //        //for consult
        //        else
        //        {
        //            request.Status = 6;
        //            _context.Requests.Update(request);
        //        }
        //        _context.SaveChanges();
        //    }
        //    return RedirectToAction("ProviderDashboard", "Provider");
        //}

        //public void HousecallConcluded(int Requestid)
        //{
        //    Request request = _context.Requests.FirstOrDefault(x => x.Requestid == Requestid);
        //    if (request != null)
        //    {
        //        request.Status = 6;
        //    }
        //    _notyf.Information("Request concluded");
        //}
        #endregion

        #region SCHEDULING
        public IActionResult ProviderSiteProviderScheduling()
        {
            return View("ProviderSiteProviderScheduling");
        }

        public Physician? GetPhysician()
        {
            try
            {
                var getPhysician = HttpContext.Session.GetString("AspnetuserId");
                var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
                Physician? physician = new();
                if (Physicianid?.Physicianid != null)
                {
                    physician = _context.Physicians.Where(x => x.Physicianid == Physicianid.Physicianid).FirstOrDefault();
                }
                return physician;
            }
            catch
            {
                _notyf.Error("Exception in Getting Physician");
                return null;
            }
        }

        [HttpGet]
        public List<Region> PhysicianRegionResults()
        {
            var getPhysician = HttpContext.Session.GetString("AspnetuserId");
            var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
            List<Region> regions = new List<Region>();
            if (Physicianid != null)
            {
                int[] physicianRegions = _context.Physicianregions.Where(x => x.Physicianid == Physicianid.Physicianid).Select(x => x.Regionid).ToArray();
                if (Physicianid?.Physicianid != null)
                {
                    regions = _context.Regions.Where(x => (physicianRegions.Any(y => y == x.Regionid))).ToList();
                }
            }
            return regions;
        }

        public List<EventsViewModel> GetPhysicianEvents()
        {
            var getPhysician = HttpContext.Session.GetString("AspnetuserId");
            var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
            var events = _adminActions.ListOfEvents();
            if (Physicianid?.Physicianid != null)
            {
                events = events.Where(x => x.ResourceId == Physicianid.Physicianid).ToList();
            }
            return events;
        }

        public IActionResult CreateShift(string physicianRegions, DateOnly StartDate, TimeOnly Starttime, TimeOnly Endtime, string Isrepeat, string checkWeekday, string Refill)
        {
            try
            {
                var getPhysician = HttpContext.Session.GetString("AspnetuserId");
                var Physicianid = _context.Physicians.FirstOrDefault(phy => phy.Aspnetuserid == getPhysician);
                bool shiftExists = _context.Shiftdetails.Any(sd => sd.Isdeleted != true && sd.Shift.Physicianid == Physicianid.Physicianid &&
                   sd.Shiftdate.Date == StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)).Date &&
                   ((Starttime <= sd.Endtime && Starttime >= sd.Starttime)
                        || (Endtime >= sd.Starttime && Endtime <= sd.Endtime)
                        || (sd.Starttime <= Endtime && sd.Starttime >= Starttime)
                        || (sd.Endtime >= Starttime && sd.Endtime <= Endtime)));
                if (!shiftExists)
                {
                    Shift shift = new();
                    shift.Physicianid = (int)Physicianid.Physicianid;
                    shift.Startdate = StartDate;
                    if (Isrepeat != null && Isrepeat == "checked")
                    {
                        shift.Isrepeat = true;
                    }
                    else
                    {
                        shift.Isrepeat = false;
                    }
                    if (string.IsNullOrEmpty(Refill))
                    {
                        shift.Repeatupto = 0;
                    }
                    else
                    {
                        shift.Repeatupto = int.Parse(Refill);
                    }
                    shift.Createddate = DateTime.Now;
                    var id = HttpContext.Session.GetString("AspnetuserId");
                    if (id != null)
                    {
                        shift.Createdby = id;
                    }
                    _context.Add(shift);
                    _context.SaveChanges();

                    Shiftdetail shiftdetail = new();
                    shiftdetail.Shiftid = shift.Shiftid;
                    shiftdetail.Shiftdate = StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                    shiftdetail.Regionid = int.Parse(physicianRegions);
                    shiftdetail.Starttime = Starttime;
                    shiftdetail.Endtime = Endtime;
                    shiftdetail.Status = 0;
                    shiftdetail.Lastrunningdate = StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                    shiftdetail.Isdeleted = false;

                    _context.Add(shiftdetail);
                    _context.SaveChanges();

                    Shiftdetailregion shiftdetailregion = new();
                    shiftdetailregion.Shiftdetailid = shiftdetail.Shiftdetailid;
                    shiftdetailregion.Regionid = int.Parse(physicianRegions);

                    _context.Add(shiftdetailregion);
                    _context.SaveChanges();

                    if (shift.Isrepeat)
                    {
                        var stringArray = checkWeekday.Split(",");
                        foreach (var weekday in stringArray)
                        {
                            // Calculate the start date for the current weekday
                            DateTime startDateForWeekday = StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)).AddDays((7 + int.Parse(weekday) - (int)StartDate.DayOfWeek) % 7);

                            // Check if the calculated start date is greater than the original start date
                            if (startDateForWeekday < StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)))
                            {
                                startDateForWeekday = startDateForWeekday.AddDays(7); // Add 7 days to move it to the next occurrence
                            }

                            // Iterate over Refill times
                            for (int i = 0; i < shift.Repeatupto; i++)
                            {
                                bool shiftdetailExists = _context.Shiftdetails.Any(sd => sd.Shift.Physicianid == Physicianid.Physicianid &&
                   sd.Shiftdate.Date == startDateForWeekday.Date &&
                   (sd.Starttime <= Endtime ||
                   sd.Endtime >= Starttime));
                                if (!shiftdetailExists)
                                {
                                    // Create a new ShiftDetail instance for each occurrence
                                    Shiftdetail shiftDetail = new Shiftdetail
                                    {
                                        Shiftid = shift.Shiftid,
                                        Shiftdate = startDateForWeekday.AddDays(i * 7), // Add i * 7 days to get the next occurrence
                                        Regionid = int.Parse(physicianRegions),
                                        Starttime = Starttime,
                                        Endtime = Endtime,
                                        Status = 0,
                                        Isdeleted = false
                                    };

                                    // Add the ShiftDetail to the database context
                                    _context.Add(shiftDetail);
                                    _context.SaveChanges();

                                    Shiftdetailregion shiftdetailregion1 = new();
                                    shiftdetailregion1.Shiftdetailid = shiftDetail.Shiftdetailid;
                                    shiftdetailregion1.Regionid = int.Parse(physicianRegions);

                                    _context.Add(shiftdetailregion1);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _notyf.Error("Shift for physician already exists in the selected interval");
                                }
                            }
                        }
                    }
                    _notyf.Success("Shift Created");
                    return Json(new { success = true, successMessage = TempData["shiftCreated"], errorMessage = TempData["shiftError"] });
                }
                else
                {
                    _notyf.Error("Shift already exists for the physician in the time interval");
                    return Json(new { success = true, successMessage = "", errorMessage = TempData["shiftError"] });
                }
            }
            catch
            {
                _notyf.Error("Exception in Creating shift for the physician.");
                return RedirectToAction("ProviderDashboard");
            }
        }
        #endregion SCHEDULING

        #region GOOD TO HAVE FEATURES
        public IActionResult ProviderInvoicing()
        {
            return View();
        }
        public IActionResult ProviderTimesheetView(DateTime startdateiso)
        {
            try
            {
                var id = HttpContext.Session.GetString("AspnetuserId");
                var physician = _context.Physicians.FirstOrDefault(physician => physician.Aspnetuserid == id);
                DateOnly startDate = DateOnly.FromDateTime(startdateiso.ToLocalTime());
                DateOnly endDate = new DateOnly();
                if (startDate.Day < 15)
                {
                    startDate = new DateOnly(startDate.Year, startDate.Month, 1);
                    endDate = new DateOnly(startDate.Year, startDate.Month, 14);
                }
                else
                {
                    startDate = new DateOnly(startDate.Year, startDate.Month, 15);
                    endDate = new DateOnly(startDate.Year, startDate.Month, 1).AddMonths(1).AddDays(-1);
                }

                Timesheet? getTimesheet = _context.Timesheets.FirstOrDefault(record => record.PhysicianId == physician.Physicianid && record.StartDate == startDate && record.EndDate == endDate);

                if (getTimesheet == null)
                {
                    DateOnly loopDate = startDate;

                    List<TimesheetDataViewModel> newTimesheet = new();
                    while (loopDate <= endDate)
                    {
                        TimesheetDataViewModel record = new()
                        {
                            TimesheetDates = loopDate,
                            TimesheetDetailId = 0
                        };

                        newTimesheet.Add(record);
                        loopDate = loopDate.AddDays(1);
                    }

                    TimesheetViewModel timesheetModel = new()
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        TimesheetData = newTimesheet,
                    };
                    return View(timesheetModel);
                }
                else
                {
                    List<TimesheetDetail> timesheetDetailsOfPhysician = _context.TimesheetDetails.Where(timesheetDetail => timesheetDetail.TimesheetId == getTimesheet.TimesheetId).ToList();

                    List<TimesheetDataViewModel> getExistingTimesheetDetails = new();
                    for (int i = 0; i < timesheetDetailsOfPhysician.Count; i++)
                    {
                        TimesheetDataViewModel model = new()
                        {
                            TimesheetDates = timesheetDetailsOfPhysician[i].TimesheetDate,
                            NoOfHouseCalls = timesheetDetailsOfPhysician[i].NumberOfHouseCall,
                            NoOfPhoneConsults = timesheetDetailsOfPhysician[i].NumberOfPhoneCall,
                            IsHoliday = timesheetDetailsOfPhysician[i].IsWeekend ?? false,
                            TotalWorkingHours = timesheetDetailsOfPhysician[i].TotalHours,
                            TimesheetDetailId = timesheetDetailsOfPhysician[i].TimesheetDetailId
                        };
                        getExistingTimesheetDetails.Add(model);

                    }

                    TimesheetViewModel timesheetModel = new()
                    {
                        TimesheetId = getTimesheet.TimesheetId,
                        StartDate = startDate,
                        EndDate = endDate,
                        TimesheetData = getExistingTimesheetDetails,
                    };

                    return View(timesheetModel);
                }
            }
            catch
            {
                _notyf.Error("Exception in ProviderTimesheetView");
                return RedirectToAction("ProviderInvoicing");
            }
        }
        [HttpPost]
        public IActionResult ProviderTimesheetView(TimesheetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var physicianAspnetuserID = HttpContext.Session.GetString("AspnetuserId");
                var Physician = _context.Physicians.FirstOrDefault(physician => physician.Aspnetuserid == physicianAspnetuserID);
                if (model.TimesheetId == 0)
                {
                    Timesheet newTimesheet = new()
                    {
                        PhysicianId = Physician.Physicianid,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        CreatedBy = Physician.Aspnetuserid,
                        CreatedDate = DateTime.Now
                    };
                    _context.Timesheets.Add(newTimesheet);
                    _context.SaveChanges();

                    for (int i = 0; i < model.TimesheetData.Count; i++)
                    {
                        TimesheetDetail timesheetdetails = new()
                        {
                            TimesheetId = newTimesheet.TimesheetId,
                            TotalHours = model.TimesheetData[i].TotalWorkingHours,
                            IsWeekend = model.TimesheetData[i].IsHoliday,
                            NumberOfHouseCall = model.TimesheetData[i].NoOfHouseCalls,
                            NumberOfPhoneCall = model.TimesheetData[i].NoOfPhoneConsults,
                            CreatedBy = Physician.Aspnetuserid,
                            CreatedDate = DateTime.Now,
                            TimesheetDate = model.TimesheetData[i].TimesheetDates
                        };
                        _context.TimesheetDetails.Add(timesheetdetails);
                    }
                    _context.SaveChanges();
                    
                    return RedirectToAction("ProviderInvoicing");
                }
                else
                {
                    for (int i = 0; i < model.TimesheetData.Count; i++)
                    {
                        TimesheetDetail timesheetDetails = _context.TimesheetDetails.FirstOrDefault(timesheet => timesheet.TimesheetDetailId == model.TimesheetData[i].TimesheetDetailId);
                        if (timesheetDetails != null)
                        {
                            timesheetDetails.TotalHours = model.TimesheetData[i].TotalWorkingHours;
                            timesheetDetails.NumberOfHouseCall = model.TimesheetData[i].NoOfHouseCalls;
                            timesheetDetails.NumberOfPhoneCall = model.TimesheetData[i].NoOfPhoneConsults;
                            timesheetDetails.IsWeekend = model.TimesheetData[i].IsHoliday;
                            timesheetDetails.ModifiedBy = Physician.Aspnetuserid;
                            timesheetDetails.ModifiedDate = DateTime.Now;
                            _context.TimesheetDetails.Update(timesheetDetails);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("ProviderInvoicing");
        }
        #endregion

    }
}

