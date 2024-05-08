using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.IAdminRecords
{
    public interface IBlockHistory
    {
        public List<BlockHistoryViewModel> GetBlockHistoryData(string FirstName, string LastName, string Email, string PhoneNo);
        public void UnblockBlockedRequest(int requestid);


    }
}
