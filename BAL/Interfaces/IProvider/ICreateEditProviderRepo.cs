using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.IProvider
{
    public interface ICreateEditProviderRepo
    {
        public Physician AddNewPhysicianDetails(EditPhysicianViewModel Model);
        public EditPhysicianViewModel GetPhysicianDetailsForEdit(int PhysicianId);
        public EditPhysicianViewModel ProviderDashboardGetPhysicianDetailsForEditPro(int PhysicianId);


    }
}
