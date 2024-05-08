using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IPasswordHasher
    {
        public string GenerateSHA256(string input);
        public string GenerateConfirmationNumber(User user);
    }
}
