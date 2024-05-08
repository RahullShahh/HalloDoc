using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IEncounterForm
    {
        public void EncounterFormPost(int requestid, EncounterFormViewModel encoModel);
        public EncounterFormViewModel EncounterFormGet(int requestid);
    }
}
