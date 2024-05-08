using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class HelperMethodsRepo : IHelperMethodsRepo
    {
        private readonly ApplicationDbContext _context;
        public HelperMethodsRepo(ApplicationDbContext context)
        {
            _context = context; 
        }

        public List<Physician> GetPhysicianFromRegionId(int regionId)
        {
            var physicians = _context.Physicians.Where(x => x.Regionid == regionId).ToList();
            return physicians;
        }
    }
}
