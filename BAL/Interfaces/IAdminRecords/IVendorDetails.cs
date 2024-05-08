using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.IAdminRecords
{
    public interface IVendorDetails
    {
        public VendorDetailsViewModel GetVendorDetails();
        public VendorDetailsViewModel GetFilteredDataForVendors(string filterSearch, int filterProfession);
        public void ChangeVendorStatusToDeleted(int vendorId);
        public void AddNewBusiness(CreateUpdateVendorViewModel model);

        public CreateUpdateVendorViewModel GetBusinessDetailsForEdit(int id);
        public CreateUpdateVendorViewModel UpdateBusinessDetails(CreateUpdateVendorViewModel model);

    }
}
