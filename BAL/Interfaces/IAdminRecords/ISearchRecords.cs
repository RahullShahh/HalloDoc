using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.IAdminRecords
{
    public interface ISearchRecords
    {
        public List<SearchRecordsTableViewModel> GetPatientDataForSearchRecords(int requestStatus, string patientName, int requestType, string phoneNumber, DateTime? fromDateOfService, DateTime? toDateOfService, string providerName, string patientEmail);
        public SearchRecordViewModel GetSearchRecordsData();
    }
}
