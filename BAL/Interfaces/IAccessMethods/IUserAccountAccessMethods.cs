using DAL.ViewModels;

namespace BAL.Interfaces.IAccessMethods
{
    public  interface IUserAccountAccessMethods
    {
        public UserAccessModel UserAccess(int accountType);

    }
}
    