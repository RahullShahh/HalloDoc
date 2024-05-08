using DAL.ViewModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IPatientDashboard
    {
        public PatientProfileViewModel PatientProfile(string email);
        public void EditProfile(PatientProfileViewModel ppm, string email);
        public ViewDocumentsViewModel ViewPatientDocsGet(int requestid, string email);
        public ViewDocumentsViewModel ViewPatientDocsPost(ViewDocumentsViewModel vm, string path);
        public PatientDashboardViewModel PatientDashboard(string email);
    }
}
