using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Repository
{
    public class AdminRepo : IAdmin
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly INotyfService _notyf;
        public AdminRepo(ApplicationDbContext context, IPasswordHasher passwordHasher,INotyfService notyf)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _notyf = notyf;
        }
        public AdminProfileViewModel AdminProfileGet(string email)
        {
            Aspnetuser user = _context.Aspnetusers.FirstOrDefault(x => x.Email == email);
            Admin admin = _context.Admins.FirstOrDefault(x => x.Email == email);
            var adminRegions = _context.Adminregions.Where(region => region.Adminid == admin.Adminid).ToList().Select(x => x.Regionid ?? 0).ToList();

            AdminProfileViewModel model = new AdminProfileViewModel()
            {
                States = _context.Regions.ToList(),
                statuses = _context.Statuses.ToList(),
                roles = _context.Roles.ToList(),
                username = user.Username,
                email = email,
                firstname = admin.Firstname,
                confirmEmail = admin.Email,
                lastname = admin.Lastname,
                phoneNo = admin.Mobile,
                address1 = admin.Address1,
                address2 = admin.Address2,
                city = admin.City,
                zipcode = admin.Zip,
                regionId = (int)admin.Regionid,
                billingPhone = admin.Altphone,
                adminId = admin.Adminid,
                statusId = (int)admin.Status,
                roleId = (int)admin.Roleid,
                selectedLocations = _context.Adminregions.FirstOrDefault(x => x.Adminid == admin.Adminid),
                ResidentialRegion = _context.Admins.FirstOrDefault(x => x.Adminid == admin.Adminid),
                chosenLocations = adminRegions
            };
            return model;
        }
        public void BillingInfoPost(AdminProfileViewModel apvm)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Adminid == apvm.adminId);
            var location = _context.Regions.FirstOrDefault(x => x.Regionid == apvm.regionId);
            if (admin != null)
            {
                admin.Address1 = apvm.address1;
                admin.Address2 = apvm.address2;
                admin.City = apvm.city;
                admin.Zip = apvm.zipcode;
                admin.Altphone = apvm.billingPhone;
                admin.Regionid = apvm.regionId;
                _context.Admins.Update(admin);
            }
            _context.SaveChanges();
        }
        public void AdminInfoPost(AdminProfileViewModel apvm, string[] AdminLocations)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Adminid == apvm.adminId);
            var location = _context.Regions.FirstOrDefault(x => x.Regionid == apvm.regionId);
            if (admin != null)
            {
                admin.Firstname = apvm.firstname;
                admin.Lastname = apvm.lastname;
                admin.Mobile = apvm.phoneNo;
                admin.Status = (short)apvm.statusId;
                admin.Roleid = apvm.roleId;
                _context.Admins.Update(admin);
            }
            _context.SaveChanges();

            if (AdminLocations != null)
            {
                foreach (var item in AdminLocations)
                {
                    Adminregion adminRegion = new Adminregion();
                    adminRegion.Adminid = admin.Adminid;
                    adminRegion.Regionid = int.Parse(item);
                    _context.Add(adminRegion);
                    _context.SaveChanges();
                }
            }
        }
        public void PasswordPost(AdminProfileViewModel apvm, string email)
        {
            var FindUser = _context.Aspnetusers.FirstOrDefault(x => x.Email == email);
            if (FindUser != null)
                FindUser.Passwordhash = _passwordHasher.GenerateSHA256(apvm.password);
            _notyf.Success("Password Updated");
        }

        public void CreateAdminAccountPost(CreateAdminViewModel profile, string[] regions)
        {
            Aspnetuser aspnetUser = new Aspnetuser();
            Guid id = Guid.NewGuid();
            
            aspnetUser.Id = id.ToString();
            aspnetUser.Username = profile.UserName;
            aspnetUser.Email = profile.Email;
            aspnetUser.Passwordhash = _passwordHasher.GenerateSHA256(profile.Password);
            aspnetUser.Phonenumber = profile.PhoneNumAspNetUsers;
            aspnetUser.Createddate = DateTime.Now;

            _context.Aspnetusers.Add(aspnetUser);
            _context.SaveChanges();

            Admin admin = new Admin();
            admin.Aspnetuserid = aspnetUser.Id;
            admin.Firstname = profile.FirstName;
            admin.Lastname = profile.LastName;
            admin.Email = profile.Email;
            admin.Mobile = profile.PhoneNumAspNetUsers;
            admin.Address1 = profile.Address1;
            admin.Address2 = profile.Address2;
            admin.Regionid = profile.state;
            admin.City = profile.City;
            admin.Zip = profile.zip;
            admin.Createddate = DateTime.Now;
            admin.Status = 1;
            admin.Createdby = aspnetUser.Id;
            admin.Modifiedby = aspnetUser.Id;
            admin.Roleid = 8;
            _context.Admins.Add(admin);
            _context.SaveChanges();

            Aspnetuserrole aspnetUserRole = new Aspnetuserrole();
            aspnetUserRole.Userid = admin.Aspnetuserid;
            aspnetUserRole.Name = "1";
            _context.Aspnetuserroles.Add(aspnetUserRole);
            _context.SaveChanges();

            if (regions != null)
            {
                foreach (var item in regions)
                {
                    Adminregion adminRegion = new Adminregion();
                    adminRegion.Adminid = admin.Adminid;
                    adminRegion.Regionid = int.Parse(item);
                    _context.Add(adminRegion);
                    _context.SaveChanges();
                }
            }
        }

    }
}
