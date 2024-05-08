using BAL.Interfaces.IAccessMethods;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Repository.AccessMethodsRepo
{
    public  class UserAccountAccessMethodsRepo:IUserAccountAccessMethods
    {
        private readonly ApplicationDbContext _context;
        public UserAccountAccessMethodsRepo(ApplicationDbContext context)
        {
            _context= context;
        }

        public UserAccessModel UserAccess(int accountType)
        {

            var aspRole = _context.Aspnetroles.ToList();

            UserAccessModel model = new UserAccessModel();
            model.Aspnetroles = aspRole;

            if (accountType == 0)
            {
                var adminList = from asp in _context.Aspnetusers
                                join ad in _context.Admins on asp.Id equals ad.Aspnetuserid
                                select new UserListViewModel
                                {
                                    AccountType = "Admin",
                                    AccountPOC = asp.Username,
                                    Phone = ad.Mobile,
                                    Status = "STATUS",
                                    OpenRequest = 0,
                                    accountType = 1,
                                    id = ad.Adminid
                                };


                var physicianList = from asp in _context.Aspnetusers
                                    join phy in _context.Physicians on asp.Id equals phy.Aspnetuserid
                                    where phy.Isdeleted != true
                                    select new UserListViewModel
                                    {
                                        AccountType = "Physician",
                                        AccountPOC = asp.Username,
                                        Phone = phy.Mobile,
                                        Status = "STATUS",
                                        OpenRequest = 0,
                                        accountType = 2,
                                        id = phy.Physicianid
                                    };

                IEnumerable<UserListViewModel> x = adminList.ToList();
                IEnumerable<UserListViewModel> y = physicianList.ToList();
                IEnumerable<UserListViewModel> mergedList = x.Union(y);

                model.UserList = mergedList.ToList();

            }
            if (accountType == 1)
            {
                var listOfUsers = from asp in _context.Aspnetusers
                                  join ad in _context.Admins on asp.Id equals ad.Aspnetuserid
                                  select new UserListViewModel
                                  {
                                      AccountType = "Admin",
                                      AccountPOC = asp.Username,
                                      Phone = ad.Mobile,
                                      Status = "STATUS",
                                      OpenRequest = 0,
                                      accountType = 1,
                                      id = ad.Adminid
                                  };

                model.UserList = listOfUsers.ToList();
            }
            if (accountType == 2)
            {
                var listOfUsers = from asp in _context.Aspnetusers
                                  join phy in _context.Physicians on asp.Id equals phy.Aspnetuserid
                                  where phy.Isdeleted !=true
                                  select new UserListViewModel
                                  {
                                      AccountType = "Physician",
                                      AccountPOC = asp.Username,
                                      Phone = phy.Mobile,
                                      Status = "STATUS",
                                      OpenRequest = 0,
                                      accountType = 2,
                                      id = phy.Physicianid
                                  };

                model.UserList = listOfUsers.ToList();
            }
            return model;
        }

    }
}
