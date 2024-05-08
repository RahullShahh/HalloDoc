using DAL.ViewModels;
using DAL.DataModels;
using Microsoft.AspNetCore.Mvc;
using DAL.DataContext;
using System.Text;
using BAL.Interfaces;
using ClosedXML.Excel;
using Rotativa.AspNetCore;
using System.Text.Json.Nodes;
using static HalloDoc_Project.Extensions.Enumerations;
using BAL.Interfaces.InterfaceProviderLocation;
using Microsoft.EntityFrameworkCore;
using BAL.Interfaces.IAdminRecords;
using BAL.Interfaces.IProvider;
using System.Collections;
using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interfaces.IAccessMethods;
using HalloDoc_Project.Authorization;
using NuGet.Protocol.Plugins;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IAdminActions _adminActions;
        private readonly IAdminTables _adminTables;
        private readonly IFileOperations _fileOperations;
        private readonly IEncounterForm _encounterForm;
        private readonly IAdmin _admin;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IProviderLocation _providerLocation;
        private readonly IHelperMethodsRepo _helperMethods;
        private readonly ISearchRecords _searchRecords;
        private readonly IPatientHistoryPatientRecords _patientHistoryPatientRecords;
        private readonly IBlockHistory _blockHistory;
        private readonly IEmailSMSLogs _emailSMSLogs;
        private readonly IVendorDetails _vendorDetails;
        private readonly ICreateEditProviderRepo _createEditProviderRepo;
        private readonly INotyfService _notyf;
        private readonly IUserAccountAccessMethods _userAccountAccessMethods;
        private const int DEFAULT_ADMIN_ID = 8;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IEmailService emailService, IAdminTables adminTables, IAdminActions adminActions, IFileOperations fileOperations, IEncounterForm encounterForm, IAdmin admin, IPasswordHasher passwordHasher, IProviderLocation providerLocation, IHelperMethodsRepo helperMethods, IPatientHistoryPatientRecords patientHistoryPatientRecords, ISearchRecords searchRecords, IBlockHistory blockHistory, IEmailSMSLogs emailSMSLogs, IVendorDetails vendorDetails, ICreateEditProviderRepo createEditProviderRepo, INotyfService notyf, IUserAccountAccessMethods userAccountAccessMethods)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _emailService = emailService;
            _adminActions = adminActions;
            _adminTables = adminTables;
            _fileOperations = fileOperations;
            _encounterForm = encounterForm;
            _admin = admin;
            _passwordHasher = passwordHasher;
            _providerLocation = providerLocation;
            _helperMethods = helperMethods;
            _patientHistoryPatientRecords = patientHistoryPatientRecords;
            _searchRecords = searchRecords;
            _blockHistory = blockHistory;
            _emailSMSLogs = emailSMSLogs;
            _vendorDetails = vendorDetails;
            _createEditProviderRepo = createEditProviderRepo;
            _notyf = notyf;
            _userAccountAccessMethods = userAccountAccessMethods;
        }

        #region LOAD ADMIN DASHBOARD
        public IActionResult AdminDashboard()
        {
            try
            {
                var email = HttpContext.Session.GetString("Email");
                AdminDashboardViewModel advm = _adminTables.AdminDashboard(email);
                return View(advm);
            }
            catch
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("jwt");
                _notyf.Error("Exception in admin dashboard-TRY AGAIN LATER");
                return RedirectToAction("login_page", "Guest");
            }
        }
        #endregion
        #region PROVIDER LOCATION
        public IActionResult ProviderLocation()
        {
            try
            {
                ProviderLocationViewModel model = _providerLocation.GetProviderLocationCoordinates();
                return View("ProviderLocation", model);
            }
            catch
            {
                _notyf.Error("Exception in Provider Location");
                return RedirectToAction("AdminDashboard");
            }
        }

        //Below method is for getting physician coordinates based on their input address but API Url is not working 
        public async Task<string> GetLatitudeLongitude(EditPhysicianViewModel model)
        {
            string state = _context.Regions.FirstOrDefault(x => x.Regionid == model.Regionid).Name;
            using (var client = new HttpClient())
            {
                string apiKey = _config["Maps:GeocodingAPIkey"];
                string baseUrl = $"https://geocode.maps.co/search?street={model.Address1 + model.Address2}&city={model.City}&state={state}&postalcode={model.ZipCode}&country=India&api_key=" + apiKey;
                //HTTP GET

                var responseTask = client.GetAsync(baseUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();

                    var json = JsonArray.Parse(content);

                    string? latitude = json?[0]?["lat"]?.ToString();
                    string? longitude = json?[0]?["lon"]?.ToString();
                }
                else
                {
                    //log response status here
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return "";
        }
        #endregion

        #region SCHEDULING

        public IActionResult ProviderScheduling()
        {
            try
            {
                var region = _context.Regions.ToList();
                ViewBag.regions = region;
                return View("ProviderViews/ProviderScheduling");
            }
            catch
            {
                _notyf.Error("Exception in Provider Scheduling");
                return RedirectToAction("AdminDashboard");
            }
        }
        public List<Region> RegionResults()
        {
            List<Region> results = _context.Regions.ToList();
            return results;
        }

        public List<Physician> GetPhysicianShift(int? region)
        {
            try
            {
                if (region == null)
                {
                    List<Physician> physicians = _context.Physicians.ToList();
                    return physicians;
                }
                else
                {
                    List<Physician> physicians = (from physicianRegions in _context.Physicianregions
                                                  join physician in _context.Physicians on
                                                  physicianRegions.Physicianid equals physician.Physicianid
                                                  where physicianRegions.Regionid == region
                                                  select physician).Distinct().ToList();
                    return physicians;
                }
            }
            catch
            {
                _notyf.Error("Exception in Provider Scheduling");
                return null;
            }
        }

        public IActionResult CreateShift(string regionDropdown, string physicianSelect, DateOnly StartDate, TimeOnly Starttime, TimeOnly Endtime, string Isrepeat, string checkWeekday, string Refill)
        {

            try
            {
                bool shiftExists = _context.Shiftdetails.Any(sd => sd.Isdeleted != true && sd.Shift.Physicianid == int.Parse(physicianSelect) &&
                   sd.Shiftdate.Date == StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)).Date &&
                   ((Starttime <= sd.Endtime && Starttime >= sd.Starttime)
                        || (Endtime >= sd.Starttime && Endtime <= sd.Endtime)
                        || (sd.Starttime <= Endtime && sd.Starttime >= Starttime)
                        || (sd.Endtime >= Starttime && sd.Endtime <= Endtime)));
                if (!shiftExists)
                {

                    //shift creation started
                    Shift shift = new();
                    shift.Physicianid = int.Parse(physicianSelect);
                    shift.Startdate = StartDate;
                    if (Isrepeat != null && Isrepeat == "checked")
                    {
                        shift.Isrepeat = true;
                    }
                    else
                    {
                        shift.Isrepeat = false;
                    }

                    shift.Repeatupto = int.Parse(Refill ?? "0");
                    shift.Createddate = DateTime.Now;
                    var email = HttpContext.Session.GetString("Email");
                    if (email != null)
                    {
                        shift.Createdby = _context.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault() ?? "";
                    }
                    _context.Add(shift);
                    _context.SaveChanges();

                    Shiftdetail shiftdetail = new();
                    shiftdetail.Shiftid = shift.Shiftid;
                    shiftdetail.Shiftdate = StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                    shiftdetail.Regionid = int.Parse(regionDropdown);
                    shiftdetail.Starttime = Starttime;
                    shiftdetail.Endtime = Endtime;
                    shiftdetail.Status = 1;
                    shiftdetail.Lastrunningdate = StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                    shiftdetail.Isdeleted = false;

                    _context.Add(shiftdetail);
                    _context.SaveChanges();

                    Shiftdetailregion shiftdetailregion = new();
                    shiftdetailregion.Shiftdetailid = shiftdetail.Shiftdetailid;
                    shiftdetailregion.Regionid = int.Parse(regionDropdown);

                    _context.Add(shiftdetailregion);
                    _context.SaveChanges();
                    //shfit created
                    if (shift.Isrepeat)
                    {
                        //repeated shift creation starts
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
                                bool shiftdetailExists = _context.Shiftdetails.Any(sd => sd.Shift.Physicianid == int.Parse(physicianSelect) &&
                   sd.Shiftdate.Date == startDateForWeekday.Date &&
                    ((Starttime <= sd.Endtime && Starttime >= sd.Starttime) // checks if new start time falls between exisiting
                        || (Endtime >= sd.Starttime && Endtime <= sd.Endtime)  // checks if new end time falls between existing)
                        || (sd.Starttime <= Endtime && sd.Starttime >= Starttime) // checks if exists start time falls between new 
                        || (sd.Endtime >= Starttime && sd.Endtime <= Endtime)));// checks if exists end time falls between new;

                                //checks if shift exists
                                if (!shiftdetailExists)
                                {
                                    // Create a new ShiftDetail instance for each occurrence
                                    Shiftdetail shiftDetail = new Shiftdetail();


                                    shiftDetail.Shiftid = shift.Shiftid;
                                    shiftDetail.Shiftdate = startDateForWeekday.AddDays(i * 7); // Add i * 7 days to get the next occurrence
                                    shiftDetail.Regionid = int.Parse(regionDropdown);
                                    shiftDetail.Starttime = Starttime;
                                    shiftDetail.Endtime = Endtime;
                                    shiftDetail.Status = 1;
                                    shiftDetail.Isdeleted = false;


                                    // Add the ShiftDetail to the database context
                                    _context.Shiftdetails.Add(shiftDetail);
                                    _context.SaveChanges();

                                    Shiftdetailregion shiftdetailregion1 = new();
                                    shiftdetailregion1.Shiftdetailid = shiftDetail.Shiftdetailid;
                                    shiftdetailregion1.Regionid = int.Parse(regionDropdown);

                                    _context.Add(shiftdetailregion1);
                                    _context.SaveChanges();
                                    //shift created
                                }
                                else
                                {
                                    _notyf.Error($"Shift for Physician exists on {startDateForWeekday} at interval {Starttime} - {Endtime}.");
                                }
                            }
                        }
                    }
                    _notyf.Success("Shift Created.");
                    return Ok();
                }
                else
                {
                    _notyf.Error("Shift already exists for the physician in the time interval");
                    return Ok();
                }

            }
            catch
            {
                _notyf.Error("Exception in creating shift");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult ShiftsReviewTablePartial(string regionid)
        {
            try
            {
                var list = _adminActions.GetRequestedShifts(regionid);
                return PartialView("ProviderViews/ShiftsReviewTablePartial", list);
            }
            catch
            {
                _notyf.Error("Exception in Shift Review Table Partial");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult ApproveMultipleShifts(int[] selectedShifts)
        {
            try
            {

                foreach (var shiftid in selectedShifts)
                {
                    Shiftdetail? shiftdetail = _context.Shiftdetails.FirstOrDefault(x => x.Shiftdetailid == shiftid);
                    if (shiftdetail != null)
                    {
                        shiftdetail.Status = (short)(shiftdetail.Status == 0 ? 1 : 0);
                        shiftdetail.Modifieddate = DateTime.Now;
                        var email = HttpContext.Session.GetString("Email");
                        shiftdetail.Modifiedby = _context.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();
                        _context.Update(shiftdetail);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                _notyf.Error("Exception in approving multiple shifts");
            }
            return Ok();
        }

        public IActionResult DeleteMultipleShifts(int[] selectedShifts)
        {
            try
            {
                foreach (var shiftid in selectedShifts)
                {
                    Shiftdetail? shiftdetail = _context.Shiftdetails.FirstOrDefault(x => x.Shiftdetailid == shiftid);
                    if (shiftdetail != null)
                    {
                        shiftdetail.Isdeleted = true;
                        shiftdetail.Modifieddate = DateTime.Now;
                        var email = HttpContext.Session.GetString("Email");
                        shiftdetail.Modifiedby = _context.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();

                        _context.Update(shiftdetail);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                _notyf.Error("Exception in deleting multiple shifts");
            }
            return Ok();
        }

        #region PROVIDERS ON CALL
        public IActionResult ProvidersOnCall()
        {
            return PartialView("ProviderViews/ProvidersOnCall");
        }
        public IActionResult ProvidersOnCallPartialTable(string providerregion)
        {
            try
            {

                var onDutyQuery = from shiftDetail in _context.Shiftdetails
                                  join phy in _context.Physicians on shiftDetail.Shift.Physicianid equals phy.Physicianid
                                  join physicianRegion in _context.Physicianregions on phy.Physicianid equals physicianRegion.Physicianid
                                  where (string.IsNullOrEmpty(providerregion) || physicianRegion.Regionid == int.Parse(providerregion)) &&
                                        shiftDetail.Shiftdate.Date == DateTime.Now.Date &&
                                        TimeOnly.FromDateTime(DateTime.Now) >= shiftDetail.Starttime &&
                                        TimeOnly.FromDateTime(DateTime.Now) <= shiftDetail.Endtime &&
                                        shiftDetail.Isdeleted != true
                                  select phy;

                var onDuty = onDutyQuery.Distinct().ToList();

                var offDutyQuery = from phy in _context.Physicians
                                   join physicianRegion in _context.Physicianregions on phy.Physicianid equals physicianRegion.Physicianid
                                   where (string.IsNullOrEmpty(providerregion) || physicianRegion.Regionid == int.Parse(providerregion)) &&
                                         !_context.Shiftdetails.Any(item => item.Shift.Physicianid == phy.Physicianid &&
                                                                            item.Shiftdate.Date == DateTime.Now.Date &&
                                                                           TimeOnly.FromDateTime(DateTime.Now) >= item.Starttime &&
                                                                           TimeOnly.FromDateTime(DateTime.Now) <= item.Endtime &&
                                                                           item.Isdeleted != true)
                                   select phy;
                var offDuty = offDutyQuery.Distinct().ToList();

                ProvidersOnCallViewModel providersOnCall = new ProvidersOnCallViewModel { OffDuty = offDuty, OnDuty = onDuty };


                return PartialView("ProviderViews/ProvidersOnCallPartialTable", providersOnCall);
            }
            catch
            {
                _notyf.Error("Exception in deleting multiple shifts");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion 

        public IActionResult ShiftsReview()
        {
            return View("ProviderViews/ShiftsReview");
        }

        //gets physicians as per selected region
        public List<Physician> PhysicianResults(int regionid)
        {
            try
            {

                List<Physician> results = (from physicianRegions in _context.Physicianregions
                                           join physician in _context.Physicians on
                                           physicianRegions.Physicianid equals physician.Physicianid
                                           where physicianRegions.Regionid == regionid
                                           select physician).Distinct().ToList();
                return results;
            }
            catch
            {
                _notyf.Error("Exception in getting physician results");
                return null;
            }


        }

        //fetches events/shifts for physicians 
        public List<EventsViewModel> GetEvents()
        {
            try
            {
                var events = _adminActions.ListOfEvents();
                return events;
            }
            catch
            {
                _notyf.Error("Exception in getting events");
                return null;
            }
        }
        //changes the status of shift from approved to pending and vice-versa.
        public List<EventsViewModel> ReturnShift(int shiftDetailId)
        {
            try
            {

                Shiftdetail? shiftdetail = _context.Shiftdetails.FirstOrDefault(x => x.Shiftdetailid == shiftDetailId);
                if (shiftdetail != null)
                {
                    shiftdetail.Status = (short)(shiftdetail.Status == 0 ? 1 : 0);
                    shiftdetail.Modifieddate = DateTime.Now;
                    var email = HttpContext.Session.GetString("Email");
                    shiftdetail.Modifiedby = _context.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();

                    _context.Update(shiftdetail);
                    _context.SaveChanges();
                }
                var list = _adminActions.ListOfEvents();
                return list;
            }
            catch
            {
                _notyf.Error("Exception in returning shift");
                return null;
            }
        }

        public List<EventsViewModel> DeleteShift(int shiftDetailId)
        {
            try
            {

                Shiftdetail? shiftdetail = _context.Shiftdetails.FirstOrDefault(x => x.Shiftdetailid == shiftDetailId);
                if (shiftdetail != null)
                {
                    shiftdetail.Isdeleted = true;
                    shiftdetail.Modifieddate = DateTime.Now;
                    var email = HttpContext.Session.GetString("Email");
                    shiftdetail.Modifiedby = _context.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();

                    _context.Update(shiftdetail);
                    _context.SaveChanges();
                }

                var list = _adminActions.ListOfEvents();
                return list;
            }
            catch
            {
                _notyf.Error("Exception in deleting shift");
                return null;
            }
        }

        public List<EventsViewModel> EditShift(string shiftDetailId, DateOnly StartDate, TimeOnly Starttime, TimeOnly Endtime)
        {
            try
            {

                Shiftdetail? shiftdetail = _context.Shiftdetails.FirstOrDefault(x => x.Shiftdetailid == int.Parse(shiftDetailId));
                if (shiftdetail != null)
                {
                    shiftdetail.Shiftdate = StartDate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
                    shiftdetail.Starttime = Starttime;
                    shiftdetail.Endtime = Endtime;
                    shiftdetail.Modifieddate = DateTime.Now;
                    var email = HttpContext.Session.GetString("Email");
                    shiftdetail.Modifiedby = _context.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();

                    _context.Update(shiftdetail);
                    _context.SaveChanges();
                }
                var list = _adminActions.ListOfEvents();
                return list;
            }
            catch
            {
                _notyf.Error("Exception in editing shift");
                return null;
            }
        }

        public IActionResult ReviewShift()
        {
            try
            {

                ShiftDetailModel shiftDetailModel = new ShiftDetailModel();
                shiftDetailModel.Regions = _context.Regions.ToList();
                return View("ProviderViews/ReviewShift", shiftDetailModel);
            }
            catch
            {
                _notyf.Error("Exception in editing shift");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult GetShifts(int region)
        {
            try
            {

                //0 for APPROVED
                //1 for Pending
                var result = (from shiftDetail in _context.Shiftdetails
                              where ((shiftDetail.Regionid == region || region == 0) &&
                                 shiftDetail.Status != 0 && shiftDetail.Isdeleted != true)
                              select new ShiftDetailModel
                              {
                                  physicianName = shiftDetail.Shift.Physician.Firstname,
                                  ShiftDetailId = shiftDetail.Shiftdetailid,
                                  day = shiftDetail.Shiftdate.ToString("MMM dd, yyyy"),
                                  starttime = shiftDetail.Starttime,
                                  endtime = shiftDetail.Endtime,
                                  Regioname = _context.Regions.FirstOrDefault(s => s.Regionid == shiftDetail.Regionid).Name,

                              }).ToList();
                return PartialView("ProviderViews/ReviewShiftPartial", result);
            }
            catch
            {
                _notyf.Error("Exception in getting shift");
                return RedirectToAction("AdminDashboard");
            }
        }

        [HttpPost]
        public IActionResult ApprovedSelected(string[] checkedValues)
        {
            try
            {
                foreach (var item in checkedValues)
                {
                    if (item != "0")
                    {
                        var status = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == int.Parse(item));
                        status.Status = 0;
                        _context.Shiftdetails.Update(status);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("ReviewShift");
            }
            catch
            {
                _notyf.Error("Exception in approved shift");
                return RedirectToAction("AdminDashboard");
            }

        }

        public IActionResult DeleteSelected(string[] checkedValues)
        {
            try
            {

                foreach (var item in checkedValues)
                {
                    if (item != "0")
                    {
                        var shiftToDelete = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == int.Parse(item));
                        shiftToDelete.Isdeleted = true;
                        _context.Shiftdetails.Update(shiftToDelete);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction("ReviewShift");
            }
            catch
            {
                _notyf.Error("Exception in deleting shift.");
                return RedirectToAction("AdminDashboard");

            }

        }

        public IActionResult MdOnCall()
        {
            try
            {
                List<Region> regions = _context.Regions.ToList();
                ViewBag.Regions = regions;
                return View("ProviderViews/MDOnCall");
            }
            catch
            {
                _notyf.Error("Exception in MDOnCall shift.");
                return RedirectToAction("AdminDashboard");
            }
        }


        public IActionResult GetPhysiciansOnCall(string region)
        {
            try
            {

                DateTime today = DateTime.Today;
                BitArray trueBitArray = new BitArray(new[] { true });
                var onDuty = (from physician in _context.Physicians
                              join shift in _context.Shifts on physician.Physicianid equals shift.Physicianid into shiftJoin
                              from shiftRecord in shiftJoin.DefaultIfEmpty()
                              join shiftDetail in _context.Shiftdetails on shiftRecord.Shiftid equals shiftDetail.Shiftid into shiftDetailJoin
                              from shiftDetailRecord in shiftDetailJoin.DefaultIfEmpty()
                              where shiftDetailRecord.Isdeleted != true && shiftDetailRecord.Shiftdate.Date == today.Date
                                                                && shiftDetailRecord.Starttime <= TimeOnly.FromDateTime(DateTime.Now)
                                                                && shiftDetailRecord.Endtime >= TimeOnly.FromDateTime(DateTime.Now)
                              select physician).Where(x => region == "0" || x.Regionid == int.Parse(region)).Distinct().ToList();

                var offDuty = _context.Physicians.Where(x => region == "0" || x.Regionid == int.Parse(region)).ToList().Except(onDuty).ToList();

                ProvidersOnCallViewModel providersOnCall = new ProvidersOnCallViewModel { OffDuty = offDuty, OnDuty = onDuty };

                return PartialView("ProviderViews/MdOnCallPartial", providersOnCall);
            }
            catch
            {
                _notyf.Error("Exception in MDOnCall shift.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region ADMIN DASHBOARD ACTIONS
        //Delete, DeleteAll, ViewUploads, SendOrders(Get) methods are not converted to three tier.

        #region Close Case Methods
        public IActionResult CloseCase(int requestid)
        {
            CloseCaseViewModel model = _adminActions.CloseCaseGet(requestid);
            return View(model);
        }
        [HttpPost]
        public IActionResult CloseCase(CloseCaseViewModel model, int requestid)
        {
            try
            {
                _adminActions.CloseCasePost(model, requestid);
                return CloseCase(requestid);
            }
            catch
            {
                _notyf.Error("Exception in Close Case Post.");
                return CloseCase(requestid);
            }
        }
        public IActionResult CloseInstance(int reqid)
        {
            try
            {
                _adminActions.ChangeRequestStatusToClosed(reqid);
                _notyf.Success("Request closed successfully.");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            catch
            {
                _notyf.Error("Exception in Close Instance action.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Assign Case Methods
        public IActionResult FilterPhysicianByRegion(int regionid)
        {
            var physicians = _helperMethods.GetPhysicianFromRegionId(regionid);
            return Json(physicians);
        }
        public Task<IActionResult> GetPhysicianForTransfer(int regionid)
        {
            var result = (from physician in _context.Physicians
                          join region in _context.Physicianregions on
                         physician.Physicianid equals region.Physicianid
                          where physician.Regionid == regionid
                          select physician).Distinct().ToList();
            return Task.FromResult<IActionResult>(Json(result));
        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, string AssignPhysician, string AssignDescription)
        {
            try
            {
                _adminActions.AssignCaseAction(RequestId, AssignPhysician, AssignDescription);
                return Ok();
            }
            catch
            {
                _notyf.Error("Exception in Assign Case Post.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Send Orders Methods
        public List<Healthprofessional> filterVenByPro(string ProfessionId)
        {
            var result = _context.Healthprofessionals.Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
            return result;
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
                _notyf.Error("Something went wrong with BusinessDataMethod");
                return Json(new { Message = "Something went wrong with BusinessDataMethod" });
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
                return View(model);
            }
            catch
            {
                _notyf.Error("Exception in send orders get.");
                return RedirectToAction("AdminDashboard");
            }
        }

        [HttpPost]
        public IActionResult SendOrders(int requestid, SendOrderViewModel sendOrder)
        {
            try
            {

                _adminActions.SendOrderAction(requestid, sendOrder);
                return SendOrders(requestid);
            }
            catch
            {
                _notyf.Error("Exception in placing the order");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Encounter Form Methods
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
                return new ViewAsPdf("EncounterFormFinalizeView", EncounterModel)
                {
                    FileName = "FinalizedEncounterForm.pdf"
                };
            }
            catch
            {
                _notyf.Error("Exception in FinalizeDownload.");
                return RedirectToAction("AdminDashboard");
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
                return RedirectToAction("AdminDashboard", "Admin");
            }
            catch
            {
                _notyf.Error("Exception in FinalizeForm.");
                return RedirectToAction("AdminDashboard");
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
                return View("EncounterForm", EncModel);
            }
            catch
            {
                _notyf.Error("Exception in Encounter form method.");
                return RedirectToAction("AdminDashboard");
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
                _notyf.Error("Exception in encounter form method.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region View Uploads Methods
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
                _notyf.Error("Exception in Delete files.");

                return RedirectToAction("AdminDashboard");
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
                _notyf.Error("Exception in Delete all files");
                return RedirectToAction("AdminDashboard");
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
                return View(uploads);
            }
            catch
            {
                _notyf.Error("Exception in View Uploads");
                return RedirectToAction("AdminDashboard");
            }
        }
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
                }
                return RedirectToAction("ViewUploads", new { requestid = uploads.RequestID });
            }
            catch
            {
                _notyf.Error("Exception in View Uploads post");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult SendMail(int requestid)
        {
            try
            {
                var path = _environment.WebRootPath;
                _emailService.SendEmailWithAttachments(requestid, path);
                return RedirectToAction("ViewUploads", "Admin", new { requestid = requestid });
            }
            catch
            {
                _notyf.Error("Exception in send mail");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Block Case Methods

        [HttpPost]
        public IActionResult BlockCase(int requestid, string blocknotes)
        {
            try
            {
                _adminActions.BlockCaseAction(requestid, blocknotes);
                return Ok();
            }
            catch
            {
                _notyf.Error("Exception in Block Case Post.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region View Case Methods
        public IActionResult ViewCase(int requestid)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    ViewCaseViewModel vc = _adminActions.ViewCaseAction(requestid);
                    return View(vc);
                }
                return View();
            }
            catch
            {
                _notyf.Error("Exception in View Case Get.");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        #region View Notes Method
        public IActionResult ViewNotes(int requestid)
        {
            try
            {

                ViewCaseViewModel vn = new ViewCaseViewModel();
                return View();
            }
            catch
            {
                _notyf.Error("Exception in View Notes get.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Cancel Case Methods

        [HttpPost]
        public ActionResult CancelCase(int requestid, string Reason, string Description)
        {
            try
            {
                _adminActions.CancelCaseAction(requestid, Reason, Description);
                return Ok();
            }
            catch
            {
                _notyf.Error("Exception in Cancel Case");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Transfer Case Methods

        [HttpPost]
        public IActionResult TransferCase(int RequestId, string TransferPhysician, string TransferDescription)
        {
            try
            {
                var email = HttpContext.Session.GetString("Email");
                var admin = _context.Admins.FirstOrDefault(x => x.Email == email);
                _adminActions.TransferCase(RequestId, TransferPhysician, TransferDescription, admin.Adminid);
                return Ok();
            }
            catch
            {
                _notyf.Error("Exception in Transfer Case");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Clear Case Methods
        [HttpPost]
        public bool ClearCaseModal(int requestid)
        {
            try
            {
                string AdminEmail = HttpContext.Session.GetString("Email");
                //Admin admin = _context.Admins.GetFirstOrDefault(a => a.Email == AdminEmail);
                return _adminActions.ClearCaseModal(requestid);
            }
            catch
            {
                _notyf.Error("Exception in clear case modal submission");
                return false;
            }
        }
        #endregion

        #region Send Agreement
        [HttpPost]
        public IActionResult SendAgreement(int RequestId, string PhoneNo, string email)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var AgreementLink = Url.Action("ReviewAgreement", "Guest", new { ReqId = RequestId }, Request.Scheme);
                    _emailService.SendAgreementLink(RequestId, AgreementLink, email);
                    return RedirectToAction("AdminDashboard", "Guest");
                }
                return View();
            }
            catch
            {
                _notyf.Error("Exception in Sending Agreement");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        #endregion

        #region ACCESS
        public IActionResult AccountAccess()
        {
            try
            {

                var roles = _context.Roles.ToList();
                AccountAccessViewModel AccountAccess = new AccountAccessViewModel()
                {
                    Roles = roles,
                };
                return View("AccessViews/AccountAccess", AccountAccess);
            }
            catch
            {
                _notyf.Error("Exception in Account Access");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult CreateRole()
        {
            try
            {

                CreateRoleViewModel RoleModel = new CreateRoleViewModel();
                RoleModel.AccountType = _context.Aspnetroles.ToList();
                return View("AccessViews/CreateRole", RoleModel);
            }
            catch
            {
                _notyf.Error("Exception in Create role.");
                return RedirectToAction("AdminDashboard");
            }
        }
        public List<Menu> MenuFilter(int AccountType)
        {
            List<Menu> menu = _context.Menus.Where(x => x.Accounttype == AccountType).ToList();
            return menu;
        }
        public bool CreateNewRole(string Rolename, int AccountType, List<int> checkboxes)
        {
            try
            {
                var AdminEmail = HttpContext.Session.GetString("Email");
                Admin admin = _context.Admins.FirstOrDefault(x => x.Email == AdminEmail);

                Role roles = new Role()
                {
                    Name = Rolename,
                    Accounttype = (short)AccountType,
                    Createdby = admin.Aspnetuserid,
                    Createddate = DateTime.Now,
                    Isdeleted = false,
                };
                _context.Roles.Add(roles);
                _context.SaveChanges();

                for (int i = 0; i < checkboxes.Count; i++)
                {
                    Rolemenu rolemenu = new Rolemenu()
                    {
                        Roleid = roles.Roleid,
                        Menuid = checkboxes[i],
                    };
                    _context.Rolemenus.Add(rolemenu);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                _notyf.Error("Exception in Creating new role method.");
                return false;
            }
        }

        public IActionResult EditRoleAccountAccess(int id)
        {
            try
            {
                EditAccessViewModel model = new EditAccessViewModel();
                var role = _context.Roles.FirstOrDefault(x => x.Roleid == id);
                List<Menu> menu = _context.Menus.Where(x => x.Accounttype == role.Accounttype).ToList();
                List<int> rolmenu = _context.Rolemenus.Where(x => x.Roleid == id).Select(x => (int)x.Menuid).ToList();
                var accountType = _context.Aspnetroles.ToList();
                model.id = id;
                model.role = role.Name;
                model.accountTypes = accountType;
                model.menu = menu;
                model.rolemenu = rolmenu;

                return View("AccessViews/EditAccess", model);
            }
            catch
            {
                _notyf.Error("Exception in Edit Role Account Access.");
                return RedirectToAction("AdminDashboard");
            }
        }
        public bool SaveEditRole(int id, string role, List<int> newPages)
        {
            try
            {

                var AdminEmail = HttpContext.Session.GetString("Email");
                Admin admin = _context.Admins.FirstOrDefault(x => x.Email == AdminEmail);

                Role roles = _context.Roles.FirstOrDefault(u => u.Roleid == id);
                List<Rolemenu> oldList = _context.Rolemenus.Where(x => x.Roleid == id).ToList();

                roles.Name = role;
                roles.Modifieddate = DateTime.Now;
                roles.Modifiedby = admin.Aspnetuserid;

                _context.Roles.Update(roles);

                for (int i = 0; i < oldList.Count; i++)
                {
                    _context.Rolemenus.Remove(oldList[i]);
                }
                for (int x = 0; x < newPages.Count; x++)
                {
                    Rolemenu rolemenu = new Rolemenu()
                    {
                        Roleid = id,
                        Menuid = newPages[x]
                    };
                    _context.Rolemenus.Add(rolemenu);
                }
                _context.SaveChanges();


                return true;
            }
            catch
            {
                _notyf.Error("Exception in Save edit role method.");
                return false;
            }
        }

        #region User Access
        public IActionResult UserAccess(int accountType)
        {
            try
            {

                UserAccessModel model = _userAccountAccessMethods.UserAccess(accountType);
                return View("AccessViews/UserAccess", model);
            }
            catch
            {
                _notyf.Error("Exception in User Access Get");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult AccountTypeFilter(int accountType)
        {
            try
            {

                UserAccessModel model = _userAccountAccessMethods.UserAccess(accountType);
                return PartialView("AccessViews/UserAccessPartialView", model);
            }
            catch
            {
                _notyf.Error("Exception in Account type filter.");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult DeleteAccountAccessRoles(int id)
        {
            try
            {

                var Adminemail = HttpContext.Session.GetString("Email");
                Admin admin = _context.Admins.FirstOrDefault(get => get.Email == Adminemail);
                Role roles = _context.Roles.FirstOrDefault(u => u.Roleid == id);
                if (roles != null)
                {

                    roles.Modifieddate = DateTime.Now;
                    roles.Modifiedby = admin.Aspnetuserid;
                    roles.Isdeleted = true;

                    _context.Roles.Update(roles);
                }
                _context.SaveChanges();
                return RedirectToAction("AccountAccess");
            }
            catch
            {
                _notyf.Error("Exception in Delete Account Access Roles");
                return RedirectToAction("AdminDashboard");
            }
        }


        #endregion

        #endregion

        #region CREATE ADMIN

        public IActionResult CreateAdminAccount()
        {
            try
            {

                CreateAdminViewModel profile = new CreateAdminViewModel();
                profile.Regions = _adminActions.GetRegionsList();
                return View("AccessViews/CreateAdminAccount", profile);
            }
            catch
            {
                _notyf.Error("Exception in Creating Admin Account.");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult CreateAdminAccount(CreateAdminViewModel profile, string[] regions)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _admin.CreateAdminAccountPost(profile, regions);
                    _notyf.Success("New Admin Created");
                    return RedirectToAction("CreateAdminAccount");
                }
                else
                {
                    profile.Regions = _adminActions.GetRegionsList();
                    _notyf.Error("Insufficient Data Provided.");
                    return View("AccessViews/CreateAdminAccount", profile);
                }
            }
            catch
            {
                _notyf.Error("Exception in Create Admin Account Post");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        #region PROVIDER

        #region Create Physician
        public IActionResult CreatePhysician()
        {
            try
            {

                EditPhysicianViewModel model = new EditPhysicianViewModel
                {
                    Role = _context.Roles.ToList(),
                    States = _context.Regions.ToList()
                };
                return View("ProviderViews/CreatePhysician", model);
            }
            catch
            {
                _notyf.Error("Exception in Create Physician get.");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult CreatePhysician(EditPhysicianViewModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    Physician Doctor = _createEditProviderRepo.AddNewPhysicianDetails(Model);
                    if (Model.SelectPhoto != null)
                    {
                        var filename = "Photo";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Doctor.Physicianid.ToString());
                        InsertFileAfterRename(Model.SelectPhoto, filepath, filename);
                        Doctor.Photo = Guid.NewGuid().ToString() + Model.SelectPhoto.Name;
                    }
                    if (Model.SelectSignature != null)
                    {
                        var filename = "Signature";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Doctor.Physicianid.ToString());
                        InsertFileAfterRename(Model.SelectSignature, filepath, filename);
                        Doctor.Signature = Guid.NewGuid().ToString() + Model.SelectSignature.Name;
                    }
                    if (Model.IndependentContractAgreement != null)
                    {
                        var filename = "ICA";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Doctor.Physicianid.ToString());
                        InsertFileAfterRename(Model.IndependentContractAgreement, filepath, filename);
                        Doctor.Isagreementdoc = true;
                    }
                    if (Model.BackgroundCheck != null)
                    {
                        var filename = "BC";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Doctor.Physicianid.ToString());
                        InsertFileAfterRename(Model.BackgroundCheck, filepath, filename);
                        Doctor.Isbackgrounddoc = true;
                    }
                    if (Model.HIPAACompliance != null)
                    {
                        var filename = "HIPPA";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Doctor.Physicianid.ToString());
                        InsertFileAfterRename(Model.HIPAACompliance, filepath, filename);
                        Doctor.Istrainingdoc = true;
                    }
                    if (Model.NonDisclosureAgreement != null)
                    {
                        var filename = "NDA";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Doctor.Physicianid.ToString());
                        InsertFileAfterRename(Model.NonDisclosureAgreement, filepath, filename);
                        Doctor.Isnondisclosuredoc = true;
                    }

                    _context.Physicians.Update(Doctor);
                    _context.SaveChanges();

                    for (int i = 0; i < Model.SelectedRegions.Count; i++)
                    {
                        var physicinRegion = new Physicianregion
                        {
                            Physicianid = Doctor.Physicianid,
                            Regionid = Model.SelectedRegions[i]
                        };
                        _context.Physicianregions.Add(physicinRegion);
                    }
                    _context.SaveChanges();
                }
                _notyf.Success("New Physician Created");
                Model.Role = _context.Roles.ToList();
                Model.States = _context.Regions.ToList();
                return View("ProviderViews/CreatePhysician", Model);
            }
            catch
            {
                _notyf.Error("Exception in Creating New Provider");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult SendEmailToProvider(String RadioButtonValue, String EmailMessage, String PhysicianId)
        {
            try
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == int.Parse(PhysicianId));
                if (RadioButtonValue == "1")
                {

                }
                if (RadioButtonValue == "2" && physician.Email != null)
                {
                    _emailService.SendEmailMessage(EmailMessage, physician.Email);
                }
                else if (RadioButtonValue == "3")
                {

                }
                return Ok();
            }
            catch
            {
                _notyf.Error("Exception in Sending Mail to Provider");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        public IActionResult ProviderPayrate(int providerId)
        {
            List<ProviderPayrateViewModel> model = (from payrateCategory in _context.PayrateCategories
                                                    join providerPayrate in _context.PayrateByProviders
                                                    on new { CategoryId = payrateCategory.PayrateCategoryId, PhysicianId = providerId } equals 
                                                    new { CategoryId = providerPayrate.PayrateCategoryId, PhysicianId=providerPayrate.Physicianid } into payrate
                                                    from payrateOfProvider in payrate.DefaultIfEmpty()
                                                    //where (payrateOfProvider == null || payrateOfProvider.Physicianid == providerId)
                                                    select new ProviderPayrateViewModel
                                                    {
                                                        PayrateId = payrateOfProvider.PayrateId,
                                                        PayrateCategoryId = payrateCategory.PayrateCategoryId,
                                                        PayrateCategoryName = payrateCategory.CategoryName,
                                                        PhysicianId = providerId,
                                                        PayrateValue = payrateOfProvider.Payrate
                                                    }).OrderBy(_=>_.PayrateCategoryName).ToList();

            return View("ProviderViews/ProviderPayrate",model);
        }

        public IActionResult SubmitProviderPayrate(int physicianId, int payrateId, int payrateCategoryId, decimal payrateValue)
        {
            try
            {

                var email = HttpContext.Session.GetString("Email");
                Admin? admin = _context.Admins.FirstOrDefault(admins => admins.Email == email);
                if (admin != null)
                {
                    if (payrateId == 0)
                    {
                        PayrateByProvider providerPayrate = new()
                        {
                            PayrateCategoryId = payrateCategoryId,
                            Physicianid = physicianId,
                            Payrate = payrateValue,
                            PayrateId = payrateId,
                            CreatedBy = admin.Aspnetuserid,
                            CreatedDate = DateTime.Now,
                        };
                        _context.PayrateByProviders.Add(providerPayrate);
                        _context.SaveChanges();
                        _notyf.Success("Payrate Updated Successfully");
                    }
                    else
                    {
                        PayrateByProvider? providerPayrate = _context.PayrateByProviders.FirstOrDefault(payrate => payrate.PayrateId == payrateId);
                        if (providerPayrate != null)
                        {
                            providerPayrate.PayrateCategoryId = payrateCategoryId;
                            providerPayrate.Physicianid = physicianId;
                            providerPayrate.PayrateId = payrateId;
                            providerPayrate.Payrate = payrateValue;
                            providerPayrate.ModifiedBy = admin.Aspnetuserid;
                            providerPayrate.ModifiedDate = DateTime.Now;

                            _context.PayrateByProviders.Update(providerPayrate);
                            _context.SaveChanges();
                            _notyf.Success("Payrate Updated Successfully");
                        }
                    }
                }
                return RedirectToAction("ProviderPayrate", new { providerId = physicianId });
            }
            catch
            {
                _notyf.Error("Exception in Submit Provider Payrate.");
                return RedirectToAction("EditPhysicianProfile", new { PhysicianId = physicianId });
            }
        }

        #region Edit Physician
        public IActionResult EditPhysicianProfile(int PhysicianId)
        {
            try
            {
                EditPhysicianViewModel EditPhysician = _createEditProviderRepo.GetPhysicianDetailsForEdit(PhysicianId);
                return View("ProviderViews/EditPhysicianProfile", EditPhysician);
            }
            catch
            {
                _notyf.Error("Exception in Edit Physician Profile");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult SubmitPhysicianAccountInfo(EditPhysicianViewModel PhysicianAccountInfo)
        {
            try
            {
                var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == PhysicianAccountInfo.PhysicianId);
                if (Physician != null)
                {
                    Physician.Status = PhysicianAccountInfo.Status;
                    //Physician.Roleid = PhysicianAccountInfo.Role;
                }
                _context.Physicians.Update(Physician);
                _context.SaveChanges();
                _notyf.Success("Details Updated Successfully.");
                return EditPhysicianProfile(PhysicianAccountInfo.PhysicianId);
            }
            catch
            {
                _notyf.Error("Exception in Submitting physician account info");
                return EditPhysicianProfile(PhysicianAccountInfo.PhysicianId);
            }
        }
        [HttpPost]
        public IActionResult SubmitPhysicianInfo(EditPhysicianViewModel PhysicianInfoModel, string[] PhysicianLocations)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //ADDING REGION ID IS REMAINING
                    var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == PhysicianInfoModel.PhysicianId);
                    if (Physician != null)
                    {
                        Physician.Firstname = PhysicianInfoModel.FirstName;
                        Physician.Lastname = PhysicianInfoModel.LastName;
                        Physician.Email = PhysicianInfoModel.Email;
                        Physician.Mobile = PhysicianInfoModel.PhoneNo;
                        Physician.Medicallicense = PhysicianInfoModel.MedicalLicense;
                        Physician.Npinumber = PhysicianInfoModel.NPINumber;
                        Physician.Syncemailaddress = PhysicianInfoModel.SyncEmail;
                    }
                    _context.Physicians.Update(Physician);
                    _context.SaveChanges();
                    if (PhysicianLocations != null)
                    {
                        foreach (var item in PhysicianLocations)
                        {
                            Physicianregion physicianLocations = new Physicianregion();

                            physicianLocations.Physicianid = Physician.Physicianid;
                            physicianLocations.Regionid = int.Parse(item);
                            _context.Physicianregions.Add(physicianLocations);
                            _context.SaveChanges();
                        }
                    }
                }
                _notyf.Success("Details Updated Successfully.");
                return EditPhysicianProfile(PhysicianInfoModel.PhysicianId);
            }
            catch
            {
                _notyf.Error("Exception in Submitting physician information");
                return EditPhysicianProfile(PhysicianInfoModel.PhysicianId);
            }
        }
        [HttpPost]
        public IActionResult SubmitPhysicianMailingBillingDetails(EditPhysicianViewModel MailingBillingModel)
        {
            try
            {
                //GetLatitudeLongitude(MailingBillingModel);
                var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == MailingBillingModel.PhysicianId);
                if (Physician != null)
                {
                    Physician.Address1 = MailingBillingModel.Address1;
                    Physician.Address2 = MailingBillingModel.Address2;
                    Physician.City = MailingBillingModel.City;
                    Physician.Regionid = MailingBillingModel.Regionid;
                    Physician.Zip = MailingBillingModel.ZipCode;
                    Physician.Altphone = MailingBillingModel.BillingPhoneNo;
                }
                _context.Physicians.Update(Physician);
                _context.SaveChanges();
                _notyf.Success("Details Updated Successfully.");
                return EditPhysicianProfile(MailingBillingModel.PhysicianId);
            }
            catch
            {
                _notyf.Error("Exception in Submitting mailing and billing detials");
                return EditPhysicianProfile(MailingBillingModel.PhysicianId);
            }
        }
        [HttpPost]
        public IActionResult SubmitProviderProfile(EditPhysicianViewModel ProviderProfile)
        {
            try
            {

                var Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == ProviderProfile.PhysicianId);
                if (Physician != null)
                {
                    Physician.Businessname = ProviderProfile.BusinessName;
                    Physician.Businesswebsite = ProviderProfile.BusinessWebsite;
                    if (ProviderProfile.SelectPhoto != null)
                    {
                        var filename = "Photo";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", ProviderProfile.PhysicianId.ToString());
                        InsertFileAfterRename(ProviderProfile.SelectPhoto, filepath, filename);
                        Physician.Photo = Guid.NewGuid().ToString() + ProviderProfile.SelectPhoto.Name;
                    }
                    if (ProviderProfile.SelectSignature != null)
                    {
                        var filename = "Signature";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", ProviderProfile.PhysicianId.ToString());
                        InsertFileAfterRename(ProviderProfile.SelectSignature, filepath, filename);
                        Physician.Signature = Guid.NewGuid().ToString() + ProviderProfile.SelectSignature.Name;
                    }
                }
                _context.Physicians.Update(Physician);
                _context.SaveChanges();
                return EditPhysicianProfile(ProviderProfile.PhysicianId);
            }
            catch
            {
                _notyf.Error("Exception in Submitting Provider Profile detials");
                return EditPhysicianProfile(ProviderProfile.PhysicianId);
            }
        }
        public IActionResult UploadOnboardingDocuments(EditPhysicianViewModel Model)
        {
            try
            {
                var PhysicianDocuments = _context.Physicians.FirstOrDefault(x => x.Physicianid == Model.PhysicianId);
                if (PhysicianDocuments != null)
                {
                    if (Model.SelectPhoto != null)
                    {
                        var filename = "Photo";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.SelectPhoto, filepath, filename);
                        PhysicianDocuments.Photo = Guid.NewGuid().ToString() + Model.SelectPhoto.Name;
                    }
                    if (Model.SelectSignature != null)
                    {
                        var filename = "Signature";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.SelectSignature, filepath, filename);
                        PhysicianDocuments.Signature = Guid.NewGuid().ToString() + Model.SelectSignature.Name;
                    }
                    if (Model.IndependentContractAgreement != null)
                    {
                        var filename = "ICA";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.IndependentContractAgreement, filepath, filename);
                        PhysicianDocuments.Isagreementdoc = true;
                    }
                    if (Model.BackgroundCheck != null)
                    {
                        var filename = "BC";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.BackgroundCheck, filepath, filename);
                        PhysicianDocuments.Isbackgrounddoc = true;
                    }
                    if (Model.HIPAACompliance != null)
                    {
                        var filename = "HIPPA";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.HIPAACompliance, filepath, filename);
                        PhysicianDocuments.Istrainingdoc = true;
                    }
                    if (Model.NonDisclosureAgreement != null)
                    {
                        var filename = "NDA";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.NonDisclosureAgreement, filepath, filename);
                        PhysicianDocuments.Isnondisclosuredoc = true;
                    }
                    if (Model.LicenseDocument != null)
                    {
                        var filename = "LD";
                        var filepath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "Providers", Model.PhysicianId.ToString());
                        InsertFileAfterRename(Model.LicenseDocument, filepath, filename);
                        PhysicianDocuments.Islicensedoc = true;
                    }
                    _context.Physicians.Update(PhysicianDocuments);
                    _context.SaveChanges();
                    _notyf.Success("Documents Uploaded Successfully.");
                }
                return EditPhysicianProfile(PhysicianDocuments.Physicianid);
            }
            catch
            {
                _notyf.Error("Exception in Uploading Onboarding Documents in edit physician profile");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        #region Insert file method for edit and create physician
        public void InsertFileAfterRename(IFormFile file, string path, string updateName)
        {
            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string[] oldfiles = Directory.GetFiles(path, updateName + ".*");
                foreach (string f in oldfiles)
                {
                    System.IO.File.Delete(f);
                }

                string extension = Path.GetExtension(file.FileName);

                string fileName = updateName + extension;

                string fullPath = Path.Combine(path, fileName);

                using FileStream stream = new(fullPath, FileMode.Create);
                file.CopyTo(stream);
            }
            catch
            {
                _notyf.Error("Exception in InsertFileAfterRename Method");
            }
        }

        #endregion

        #region Provider Menu
        public IActionResult ProviderMenu()
        {
            try
            {

                ProviderMenuViewModel ProviderMenuData = new ProviderMenuViewModel();
                ProviderMenuData.Region = _context.Regions.ToList();

                var DoctorDetails = (from p in _context.Physicians
                                     join ph in _context.Physiciannotifications on p.Physicianid equals ph.Physicianid into notigroup
                                     from notiitem in notigroup.DefaultIfEmpty()
                                     select new Providers
                                     {
                                         PhysicianId = p.Physicianid,
                                         Name = p.Firstname + " " + p.Lastname,
                                         ProviderStatus = p.Status ?? 0,
                                         Email = p.Email,
                                         Notification = notiitem.Isnotificationstopped ? true : false,
                                         Role = p.Roleid ?? 0
                                     }).ToList();
                ProviderMenuData.providers = DoctorDetails;
                return View("ProviderViews/ProviderMenu", ProviderMenuData);
            }
            catch
            {
                _notyf.Error("Exception in Provider Menu Get.");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult ProviderMenuPartial(int regionid)
        {
            try
            {
                ProviderMenuViewModel ProviderMenuData = new ProviderMenuViewModel();
                var DoctorDetails = (from p in _context.Physicians
                                     join ph in _context.Physiciannotifications on p.Physicianid equals ph.Physicianid into notigroup
                                     from notiitem in notigroup.DefaultIfEmpty()
                                     where (regionid == 0 || p.Regionid == regionid)
                                     select new Providers
                                     {
                                         PhysicianId = p.Physicianid,
                                         Name = p.Firstname + " " + p.Lastname,
                                         ProviderStatus = p.Status ?? 0,
                                         Email = p.Email,
                                         Notification = notiitem.Isnotificationstopped ? true : false,
                                         Role = p.Roleid ?? 0
                                     }).ToList();
                ProviderMenuData.providers = DoctorDetails;
                return PartialView("ProviderViews/ProviderMenuPartial", ProviderMenuData);
            }
            catch
            {
                _notyf.Error("Exception in Provider menu partial method.");
                return RedirectToAction("AdminDashboard");
            }
        }
        public void UpdateNotifications(int PhysicianId)
        {
            try
            {
                var PhysicianNotification = _context.Physiciannotifications.FirstOrDefault(x => x.Physicianid == PhysicianId);
                if (PhysicianNotification.Isnotificationstopped)
                {
                    PhysicianNotification.Isnotificationstopped = false;
                }
                else if (!PhysicianNotification.Isnotificationstopped)
                {
                    PhysicianNotification.Isnotificationstopped = true;
                }
                _context.Physiciannotifications.Update(PhysicianNotification);
                _context.SaveChanges();
                _notyf.Custom("Notifications Updated.", 5, "#9FE2BF", "fa fa-gear");
            }
            catch
            {
                _notyf.Error("Exception in Updating Notifications");
            }
        }
        #endregion

        #endregion          

        #region Miscellaneous Methods (Move these methods in extension folder)
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
        #endregion

        #region ADMIN DASHBOARD METHODS

        #region Send Link
        [HttpPost]
        public void SendLink(string FirstName, string LastName, string Email)
        {
            try
            {

                var AgreementLink = Url.Action("patient_submit_request_screen", "Guest", new { }, Request.Scheme);
                _emailService.SendEmailWithLink(FirstName, LastName, Email, AgreementLink);
                _notyf.Success("Website link sent via email.");
            }
            catch
            {
                _notyf.Error("Exception in sending email link");
            }
        }
        #endregion

        #region Export Methods
        [HttpPost]
        public byte[] ExportToExcel(int status, int page, int region, int type, string search)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("EmployeeList");

            // Add headers with yellow background color
            var headers = new string[] { "Name", "PhoneNo", "Email", "Requestid", "Status", "Address", "RequestTypeId", "UserID" };
            var headerCell = worksheet.Cell(1, 1);
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Get employee data (assuming GetEmployeeList() returns a list of employees)
            var records = ExcelFile(status, page, region, type, search);
            for (int i = 0; i < records.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = records[i].Name;
                worksheet.Cell(i + 2, 2).Value = records[i].PhoneNo;
                worksheet.Cell(i + 2, 3).Value = records[i].email;
                worksheet.Cell(i + 2, 4).Value = records[i].requestid;
                worksheet.Cell(i + 2, 5).Value = records[i].status;
                worksheet.Cell(i + 2, 6).Value = records[i].address;
                worksheet.Cell(i + 2, 7).Value = records[i].requesttypeid;
                worksheet.Cell(i + 2, 8).Value = records[i].userid;
            }

            // Prepare the response
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Return the Excel file
            return stream.ToArray();

        }
        public List<ExcelFileViewModel> ExcelFile(int dashboardstatus, int page, int region, int type, string search)
        {
            List<ExcelFileViewModel> ExcelData = new List<ExcelFileViewModel>();
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
                status = dashboardstatus
            };

            List<short> validRequestTypes = new List<short>();
            switch (filter.status)
            {
                case (int)DashboardStatus.New:
                    validRequestTypes.Add((short)RequestStatus.Unassigned);
                    break;
                case (int)DashboardStatus.Pending:
                    validRequestTypes.Add((short)RequestStatus.Accepted);
                    break;
                case (int)DashboardStatus.Active:
                    validRequestTypes.Add((short)RequestStatus.MDEnRoute);
                    validRequestTypes.Add((short)RequestStatus.MDOnSite);
                    break;
                case (int)DashboardStatus.Conclude:
                    validRequestTypes.Add((short)RequestStatus.Conclude);
                    break;
                case (int)DashboardStatus.ToClose:
                    validRequestTypes.Add((short)RequestStatus.Cancelled);
                    validRequestTypes.Add((short)RequestStatus.CancelledByPatient);
                    validRequestTypes.Add((short)RequestStatus.Closed);

                    break;
                case (int)DashboardStatus.Unpaid:
                    validRequestTypes.Add((short)RequestStatus.Unpaid);
                    break;
            }
            pagesize = 5;
            pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            ExcelData = (from r in _context.Requests
                         join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                         where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                         && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                         && (validRequestTypes.Contains(r.Status))
                         && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                         select new ExcelFileViewModel
                         {
                             requestid = r.Requestid,
                             Name = rc.Firstname + " " + rc.Lastname,
                             email = rc.Email ?? "",
                             PhoneNo = rc.Phonenumber ?? "",
                             address = rc.Address ?? "",
                             requesttypeid = r.Requesttypeid,
                             status = r.Status
                         }).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            return ExcelData;
        }
        #endregion

        #region Export-All Methods
        [HttpPost]
        public byte[] ExportAllToExcel(int status)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("EmployeeList");

            // Add headers with yellow background color
            var headers = new string[] { "Name", "PhoneNo", "Email", "Requestid", "Status", "Address", "RequestTypeId", "UserID" };
            var headerCell = worksheet.Cell(1, 1);
            var headerStyle = headerCell.Style;
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Get employee data (assuming GetEmployeeList() returns a list of employees)
            var records = ExcelFileExportAll(status);
            for (int i = 0; i < records.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = records[i].Name;
                worksheet.Cell(i + 2, 2).Value = records[i].PhoneNo;
                worksheet.Cell(i + 2, 3).Value = records[i].email;
                worksheet.Cell(i + 2, 4).Value = records[i].requestid;
                worksheet.Cell(i + 2, 5).Value = records[i].status;
                worksheet.Cell(i + 2, 6).Value = records[i].address;
                worksheet.Cell(i + 2, 7).Value = records[i].requesttypeid;
                worksheet.Cell(i + 2, 8).Value = records[i].userid;
            }

            // Prepare the response
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Return the Excel file
            return stream.ToArray();

        }
        public List<ExcelFileViewModel> ExcelFileExportAll(int dashboardstatus)
        {
            List<ExcelFileViewModel> ExcelData = new List<ExcelFileViewModel>();
            int pagesize = 5;
            int pageNumber = 1;

            DashboardFilter filter = new DashboardFilter()
            {
                status = dashboardstatus
            };

            List<short> validRequestTypes = new List<short>();
            switch (filter.status)
            {
                case (int)DashboardStatus.New:
                    validRequestTypes.Add((short)RequestStatus.Unassigned);
                    break;
                case (int)DashboardStatus.Pending:
                    validRequestTypes.Add((short)RequestStatus.Accepted);
                    break;
                case (int)DashboardStatus.Active:
                    validRequestTypes.Add((short)RequestStatus.MDEnRoute);
                    validRequestTypes.Add((short)RequestStatus.MDOnSite);
                    break;
                case (int)DashboardStatus.Conclude:
                    validRequestTypes.Add((short)RequestStatus.Conclude);
                    break;
                case (int)DashboardStatus.ToClose:
                    validRequestTypes.Add((short)RequestStatus.Cancelled);
                    validRequestTypes.Add((short)RequestStatus.CancelledByPatient);
                    validRequestTypes.Add((short)RequestStatus.Closed);

                    break;
                case (int)DashboardStatus.Unpaid:
                    validRequestTypes.Add((short)RequestStatus.Unpaid);
                    break;
            }

            ExcelData = (from r in _context.Requests
                         join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                         where (validRequestTypes.Contains(r.Status))
                         select new ExcelFileViewModel
                         {
                             requestid = r.Requestid,
                             Name = rc.Firstname + " " + rc.Lastname,
                             email = rc.Email,
                             PhoneNo = rc.Phonenumber,
                             address = rc.Address,
                             requesttypeid = r.Requesttypeid,
                             status = r.Status
                         }).ToList();
            return ExcelData;
        }
        #endregion

        #region Create Request Admin Dashboard
        public IActionResult CreateRequestAdminDashboard()
        {
            CreateRequestViewModel model = new CreateRequestViewModel()
            {
                Regions = _context.Regions.ToList(),
            };
            return View(model);
        }

        [HttpPost]
        [RoleAuthorize((int)AllowMenu.AdminDashboard)]
        public IActionResult CreateRequestAdminDashboard(CreateRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                _adminActions.CreateRequestFromAdminDashboard(model);
            }
            return RedirectToAction("CreateRequestAdminDashboard");
        }
        #endregion

        #endregion

        #region RECORDS

        #region PatientHistory and PatientRecords
        public IActionResult PatientHistoryPartialTable(string FirstName, string LastName, string Email, string PhoneNo)
        {
            try
            {

                List<PatientHistoryTableViewModel> PatientHistoryList = _patientHistoryPatientRecords.GetPatientHistoryData(FirstName, LastName, Email, PhoneNo);
                return PartialView("Records/PatienthistoryPartialTable", PatientHistoryList);
            }
            catch
            {
                _notyf.Error("Exception in Patient History.");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult PatientHistory()
        {
            return View("Records/PatientHistory");
        }
        public IActionResult PatientRecords(int Userid)
        {
            try
            {

                List<PatientRecordsViewModel> PatientRecordsList = _patientHistoryPatientRecords.GetPatientRecordsData(Userid);
                return View("Records/PatientRecords", PatientRecordsList);
            }
            catch
            {
                _notyf.Error("Exception in Patient Records");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region Search Records
        public IActionResult SelectRecordsPartialTable(int requestStatus, string patientName, int requestType, string phoneNumber, DateTime? fromDateOfService, DateTime? toDateOfService, string providerName, string patientEmail)
        {
            try
            {

                List<SearchRecordsTableViewModel> PatientRecords = _searchRecords.GetPatientDataForSearchRecords(requestStatus, patientName, requestType, phoneNumber, fromDateOfService, toDateOfService, providerName, patientEmail);
                return PartialView("Records/SearchRecordsPartialTable", PatientRecords);
            }
            catch
            {
                _notyf.Error("Exception in Select Records Partial Table");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult DeleteRequest(int requestid)
        {
            try
            {

                Request UserRequest = _context.Requests.FirstOrDefault(request => request.Requestid == requestid);
                if (UserRequest != null)
                {
                    UserRequest.Isdeleted = true;
                }
                _context.Requests.Update(UserRequest);
                _context.SaveChanges();
                return RedirectToAction("SearchRecords");
            }
            catch
            {
                _notyf.Error("Exception in Delete Requests");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult SearchRecords()
        {
            try
            {

                SearchRecordViewModel model = _searchRecords.GetSearchRecordsData();
                return View("Records/SearchRecords", model);
            }
            catch
            {
                _notyf.Error("Exception in Search Records");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion 

        #region Block History Records
        public IActionResult BlockHistory()
        {
            return View("Records/BlockHistory");
        }
        public IActionResult BlockHistoryPartialTable(string FirstName, string LastName, string Email, string PhoneNo)
        {
            try
            {

                List<BlockHistoryViewModel> BlockHistoryRecords = _blockHistory.GetBlockHistoryData(FirstName, LastName, Email, PhoneNo);
                return PartialView("Records/BlockHistoryPartialTable", BlockHistoryRecords);
            }
            catch
            {
                _notyf.Error("Exception in Block History Partial Table");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult UnblockRequest(int requestid)
        {
            try
            {
                _blockHistory.UnblockBlockedRequest(requestid);
                return RedirectToAction("BlockHistory");
            }
            catch
            {
                _notyf.Error("Exception in Unbloack Requests");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region EMAIL_LOG_RECORDS

        public IActionResult EmailLogs()
        {
            try
            {

                EmailLogViewModel emaildata = _emailSMSLogs.GetRolesEmailLogTable();
                return View("Records/EmailLog", emaildata);
            }
            catch
            {
                _notyf.Error("Exception in Email Logs");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult EmailLogPartialTable(string ReceiverName, string Email, DateTime? CreatedDate, DateTime? SentDate, int RoleId)
        {
            try
            {

                List<EmailLogViewModel> EmailList = _emailSMSLogs.GetEmailLogsData(ReceiverName, Email, CreatedDate, SentDate, RoleId);
                return PartialView("Records/EmailLogPartialTable", EmailList);
            }
            catch
            {
                _notyf.Error("Exception in Email Log Partial Table.");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region SMS LOG RECORDS
        public IActionResult SMSLogs()
        {
            try
            {

                SMSLogViewModel SMSdata = _emailSMSLogs.GetRolesSMSLogTable();
                return View("Records/SMSLog", SMSdata);
            }
            catch
            {
                _notyf.Error("Exception in Sms Logs Table");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult SMSLogPartialTable(string ReceiverName, string PhoneNo, DateTime? CreatedDate, DateTime? SentDate, int RoleId)
        {
            try
            {

                List<SMSLogViewModel> SMSList = _emailSMSLogs.GetSMSLogsData(ReceiverName, PhoneNo, CreatedDate, SentDate, RoleId);
                return PartialView("Records/SMSLogPartialTable", SMSList);
            }
            catch
            {
                _notyf.Error("Exception in Sms log Partial Table");
                return RedirectToAction("AdminDashboard");
            }
        }
        #endregion

        #region VENDOR DETAILS / CREATE VENDORS / EDIT VENDORS
        public IActionResult VendorDetails()
        {
            try
            {

                VendorDetailsViewModel model = _vendorDetails.GetVendorDetails();
                return View("Partners/VendorDetails", model);
            }
            catch
            {
                _notyf.Error("Exception in Vendor Details");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult VendorsFilter(string filterSearch, int filterProfession)
        {
            try
            {

                VendorDetailsViewModel model = _vendorDetails.GetFilteredDataForVendors(filterSearch, filterProfession);
                return PartialView("Partners/VendorDetailsPartialTable", model);
            }
            catch
            {
                _notyf.Error("Exception in Vendors Filter");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult DeleteVendor(int id)
        {
            try
            {

                _vendorDetails.ChangeVendorStatusToDeleted(id);
                return RedirectToAction("VendorDetails");
            }
            catch
            {
                _notyf.Error("Exception in Delete Vendors");
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult AddBusiness()
        {
            try
            {
                CreateUpdateVendorViewModel model = new()
                {
                    types = _context.Healthprofessionaltypes.ToList(),
                    regions = _context.Regions.ToList()
                };

                return View("Partners/AddBusiness", model);
            }
            catch
            {
                _notyf.Error("Exception in Adding business");
                return RedirectToAction("AdminDashboard");
            }
        }

        [HttpPost]

        public IActionResult AddBusiness(CreateUpdateVendorViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _vendorDetails.AddNewBusiness(model);
                    _notyf.Success("New vendor added successfully");

                }
                return RedirectToAction("VendorDetails");
            }
            catch
            {
                _notyf.Error("Exception in Adding business.");
                return RedirectToAction("AdminDashboard");
            }
        }

        public IActionResult EditBusiness(int id)
        {
            try
            {
                Console.WriteLine(HttpContext);
                CreateUpdateVendorViewModel model = _vendorDetails.GetBusinessDetailsForEdit(id);
                return View("Partners/EditBusiness", model);
            }
            catch
            {
                _notyf.Error("Exception in Edit Business");
                return RedirectToAction("AdminDashboard");
            }
        }

        [HttpPost]
        public IActionResult EditBusiness(CreateUpdateVendorViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    model = _vendorDetails.UpdateBusinessDetails(model);
                    _notyf.Success("Vendor Details saved successfully");
                }
                return RedirectToAction("EditBusiness", model.Id);
            }
            catch
            {
                _notyf.Error("Exception in Editing Business");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        #endregion

        #region ADMIN DASHBOARD TABLES

        //common method that is always called to setup the dashboard filters for the table of admin dashboard
        public DashboardFilter SetDashboardFilterValues(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
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

        [HttpPost]
        public IActionResult NewTable(int page, int region, int type, string search)
        {
            try
            {

                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.GetNewTable(filter);
                model.currentPage = filter.pageNumber;

                return PartialView("NewTable", model);
            }
            catch
            {
                _notyf.Error("Exception in New Table Admin Dashboard");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult ActiveTable(int page, int region, int type, string search)
        {
            try
            {

                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.GetActiveTable(filter);
                model.currentPage = filter.pageNumber;

                return PartialView("ActiveTable", model);
            }
            catch
            {
                _notyf.Error("Exception in Active Table Admin Dashboard");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult PendingTable(int page, int region, int type, string search)
        {
            try
            {

                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.GetPendingTable(filter);
                model.currentPage = filter.pageNumber;

                return PartialView("PendingTable", model);
            }
            catch
            {
                _notyf.Error("Exception in Pending Table Admin Dashboard");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult ConcludeTable(int page, int region, int type, string search)
        {
            try
            {

                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.GetConcludeTable(filter);
                model.currentPage = filter.pageNumber;

                return PartialView("ConcludeTable", model);
            }
            catch
            {
                _notyf.Error("Exception in Conclude Table Admin Dashboard");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult ToCloseTable(int page, int region, int type, string search)
        {
            try
            {

                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.GetToCloseTable(filter);
                model.currentPage = filter.pageNumber;

                return PartialView("ToCloseTable", model);
            }
            catch
            {
                _notyf.Error("Exception in To-CLose Table Admin Dashboard");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult UnpaidTable(int page, int region, int type, string search)
        {
            try
            {

                var filter = SetDashboardFilterValues(page, region, type, search);
                AdminDashboardViewModel model = _adminTables.GetUnpaidTable(filter);
                model.currentPage = filter.pageNumber;

                return PartialView("UnpaidTable", model);
            }
            catch
            {
                _notyf.Error("Exception in Unpaid Table Admin Dashboard");
                return RedirectToAction("AdminDashboard");
            }
        }

        #endregion

        #region ADMIN MY PROFILE

        public IActionResult AdminProfile()
        {
            try
            {
                var email = HttpContext.Session.GetString("Email");
                AdminProfileViewModel model = new();
                if (email != null)
                {
                    model = _admin.AdminProfileGet(email);
                }
                return View("AdminProfile", model);
            }
            catch
            {
                _notyf.Error("Exception in Admin Profile");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult AdminInfoPost(AdminProfileViewModel apvm, string[] AdminLocations)
        {
            try
            {
                _admin.AdminInfoPost(apvm, AdminLocations);
                _notyf.Success("Details Updated Successfully");

                return AdminProfile();
            }
            catch
            {
                _notyf.Error("Exception in Admin Information Post");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult BillingInfoPost(AdminProfileViewModel apvm)
        {
            try
            {
                _admin.BillingInfoPost(apvm);
                _notyf.Success("Details Updated Successfully");
                return AdminProfile();
            }
            catch
            {
                _notyf.Error("Exception in Billing Information Post");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult PasswordPost(AdminProfileViewModel apvm)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email != null)
                _admin.PasswordPost(apvm, email);
            return AdminProfile();
        }

        #endregion

        #region Edit Admin Profile

        public IActionResult EditAdminAccount(int id)
        {
            try
            {

                Admin? GetAdmin = _context.Admins.FirstOrDefault(admin => admin.Adminid == id);
                AdminProfileViewModel model = _admin.AdminProfileGet(GetAdmin.Email);
                return View("AccessViews/EditAdminAccount", model);
            }
            catch
            {
                _notyf.Error("Exception in Edit Admin Account.");
                return RedirectToAction("AdminDashboard");
            }

        }
        [HttpPost]
        public IActionResult EditAdminInfoPost(AdminProfileViewModel apvm, string[] AdminLocations)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _admin.AdminInfoPost(apvm, AdminLocations);
                    _notyf.Success("Admin details updated");
                }
                return EditAdminAccount(apvm.adminId);
            }
            catch
            {
                _notyf.Error("Exception in Edit Admin Info Post");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult EditBillingInfoPost(AdminProfileViewModel apvm)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _admin.BillingInfoPost(apvm);
                    _notyf.Success("Billing details updated");
                }
                return EditAdminAccount(apvm.adminId);
            }
            catch
            {
                _notyf.Error("Exception in Billing Info Post");
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpPost]
        public IActionResult EditPasswordPost(AdminProfileViewModel apvm)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var email = HttpContext.Session.GetString("Email");
                    if (email != null)
                    {
                        _admin.PasswordPost(apvm, email);
                    }
                }
                return EditAdminAccount(apvm.adminId);
            }
            catch
            {
                _notyf.Error("Exception in Password Post");
                return RedirectToAction("AdminDashboard");
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
    }
}