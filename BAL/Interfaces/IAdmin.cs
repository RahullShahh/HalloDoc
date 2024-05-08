using BAL.Repository;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IAdmin 
    {
        
        public AdminProfileViewModel AdminProfileGet(string email);
        public void AdminInfoPost(AdminProfileViewModel apvm, string[] adminLocations);
        public void BillingInfoPost(AdminProfileViewModel apvm);
        public void PasswordPost(AdminProfileViewModel apvm, string email);

        public void CreateAdminAccountPost(CreateAdminViewModel profile, string[] regions);

    }
}
