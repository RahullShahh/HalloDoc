using BAL.Interfaces.InterfaceProviderLocation;
using DAL.DataContext;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.ProviderLocationRepository
{
    public class ProviderLocationRepo : IProviderLocation
    {
        private readonly ApplicationDbContext _context;
        public ProviderLocationRepo(ApplicationDbContext context)
        {
            _context=context;
        }
        public ProviderLocationViewModel GetProviderLocationCoordinates()
        {
            IEnumerable<PhyLocationRow> list = (from pl in _context.Physicianlocations
                                                select new PhyLocationRow
                                                {
                                                    PhysicianName = pl.Physicianname??"name_not_found",
                                                    Latitude = pl.Latitude ?? 0,
                                                    Longitude = pl.Longtitude ?? 0,
                                                });

            ProviderLocationViewModel model = new ProviderLocationViewModel()
            {
                locationList = list.ToList(),
            };
            return model;
        }
    }
}
