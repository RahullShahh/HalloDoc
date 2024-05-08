using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IRequestRepo
    { 
        public void PRequest(PatientModel pm,string uniqueid,string path);
        public void BRequest(BusinessModel bm, string SetUpLink);
        public void FRequest(FamilyFriendModel ffm,string uniqueid,string path,string SetUpLink);
        public void CRequest(ConciergeModel cm, string SetUpLink);
    }
}