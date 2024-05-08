using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ProviderPayrateViewModel
    {
        public int PhysicianId { get; set; }
        public string? PayrateCategoryName { get; set; }
        public int? PayrateId { get; set; }
        public int? PayrateCategoryId { get; set; }
        public decimal? PayrateValue {  get; set; }
    }
}
