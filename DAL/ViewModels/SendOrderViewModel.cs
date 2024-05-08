using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class SendOrderViewModel
    {
        public string profession {  get; set; }
        public string business {  get; set; }
        public string BusContact {  get; set; }
        public string BusEmail {  get; set; }
        public string FaxNo { get; set; }
        public string prescription {  get; set; }
        public int RefillCount {  get; set; }
        public int requestid {  get; set; }
        public List<Healthprofessional> healthprofessionals { get; set; }
        public List<Healthprofessionaltype> healthprofessionaltype {  get; set; }
    }
}
