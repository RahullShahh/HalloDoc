using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ShiftDetailModel
    {
        public string? physicianName { get; set; }
        public int? ShiftDetailId { get; set; }
        public string? Regioname { get; set; }
        public string? day { get; set; }
        public TimeOnly? starttime { get; set; }
        public TimeOnly? endtime { get; set; }
        public List<Region> Regions { get; set; }
    }
}
