using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces.InterfaceProviderLocation
{
    public interface IProviderLocation
    {
        public ProviderLocationViewModel GetProviderLocationCoordinates();
    }
}
