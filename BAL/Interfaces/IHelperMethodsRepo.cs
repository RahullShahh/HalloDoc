using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IHelperMethodsRepo
    {
        public List<Physician> GetPhysicianFromRegionId(int regionId);
    }
}
