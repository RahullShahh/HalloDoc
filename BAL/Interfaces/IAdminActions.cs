using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BAL.Interfaces
{
    public interface IAdminActions
    {
        public ViewCaseViewModel ViewCaseAction(int requestid);
        public void AssignCaseAction(int RequestId, string AssignPhysician, string AssignDescription);
        public void CancelCaseAction(int requestid, string Reason, string Description);
        public void BlockCaseAction(int requestid, string blocknotes);
        public void TransferCase(int RequestId, string TransferPhysician, string TransferDescription,int adminid);
        public bool ClearCaseModal(int requestid);
        public void SendOrderAction(int requestid, SendOrderViewModel sendOrder);
        public CloseCaseViewModel CloseCaseGet(int requestid);
        public void CloseCasePost(CloseCaseViewModel model,int requestid);
        public void ChangeRequestStatusToClosed(int requestId);
        public void CreateRequestFromAdminDashboard(CreateRequestViewModel model);
        //public List<Scheduling> GetEvents(int region);
        //public void CreateShift(Scheduling model, string email, int physicianId);
        public void ProviderAcceptCase(int requestid);
        public List<EventsViewModel> ListOfEvents();
        public List<RequestedShiftsViewModel> GetRequestedShifts(string regionid);
        public void ProviderTransferCase(int RequestId, string TransferPhysician, string TransferDescription, int providerid);

        public List<Region> GetRegionsList();
    }
}
