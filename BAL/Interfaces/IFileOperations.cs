using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IFileOperations
    {
        public void insertfiles(IFormFile document, String _path);
        public void insertfilesunique(IFormFile document, String uniqueID,String path);
    }
}
