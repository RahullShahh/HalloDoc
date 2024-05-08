using BAL.Interfaces;
using DAL.DataContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class FileOperationsRepo : IFileOperations
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        public FileOperationsRepo(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public void insertfiles(IFormFile document, string _path)
        {
            string path = _path;
            string filePath = "Content/" + document.FileName;
            string fullPath = Path.Combine(path, filePath);
            using FileStream stream = new(fullPath, FileMode.Create);
            document.CopyTo(stream);
        }

        public void insertfilesunique(IFormFile document, string uniqueID,string path)
        {
            string filePath = "Content/" + uniqueID + "$" + document.FileName;
            string fullPath = Path.Combine(path, filePath);

            using FileStream stream = new(fullPath, FileMode.Create);
            document.CopyTo(stream);
        }
    }
}
