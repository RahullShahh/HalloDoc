using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IAgreement
    {
        public void AgreementAccepted(int Requestid);
        public void AgreementRejected(int Requestid, string Notes);
    }
}
