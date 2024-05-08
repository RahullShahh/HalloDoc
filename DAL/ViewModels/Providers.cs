using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class Providers
    {
        public string Name {  get; set; }
        public int Role {  get; set; }
        public string OnCallStatus {  get; set; }
        public int ProviderStatus {  get; set; }
        public int PhysicianId {  get; set; }
        public string Email {  get; set; }
        public bool Notification {  get; set; }
    }
}
