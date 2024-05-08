using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.IAdminRecords
{
    public interface IPatientHistoryPatientRecords
    {
        public List<PatientHistoryTableViewModel> GetPatientHistoryData(string FirstName, string LastName, string Email, string PhoneNo);
        public List<PatientRecordsViewModel> GetPatientRecordsData(int Userid);

    }
}
