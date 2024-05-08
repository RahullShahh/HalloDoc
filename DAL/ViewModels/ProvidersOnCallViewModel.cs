using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ProvidersOnCallViewModel
    {
        public List<Physician>? OnDuty { get; set; }
        public List<Physician>? OffDuty { get; set; }
    }
}
