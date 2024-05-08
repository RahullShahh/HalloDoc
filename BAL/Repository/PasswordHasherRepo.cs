using System.Text;
using System.Security.Cryptography;
using BAL.Interfaces;
using DAL.DataModels;
using DAL.DataContext;

namespace BAL.Repository
{
    public class PasswordHasherRepo : IPasswordHasher
    {
        private readonly ApplicationDbContext _context;
        
        public PasswordHasherRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        public string GenerateConfirmationNumber(User user)
        {
            string regionAbbr = _context.Regions.FirstOrDefault(region => region.Regionid == user.Regionid).Abbreviation;

            DateTime todayStart = DateTime.Now.Date;
            int count = _context.Requests.Count(req => req.Createddate > todayStart);

            string confirmationNumber = regionAbbr + user.Createddate.Day.ToString("D2") + user.Createddate.Month.ToString("D2") + user.Lastname.Substring(0, 2).ToUpper() + user.Firstname.Substring(0, 2).ToUpper() + (count + 1).ToString("D4");
            return confirmationNumber;
        }
    }
}
