using BAL.Repository;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IAdminTables
    {
        public AdminDashboardViewModel GetNewTable(DashboardFilter filter);
        public AdminDashboardViewModel GetActiveTable(DashboardFilter filter);
        public AdminDashboardViewModel GetPendingTable(DashboardFilter filter);
        public AdminDashboardViewModel GetConcludeTable(DashboardFilter filter);
        public AdminDashboardViewModel GetToCloseTable(DashboardFilter filter);
        public AdminDashboardViewModel GetUnpaidTable(DashboardFilter filter);
        public AdminDashboardViewModel AdminDashboardView();
        public AdminDashboardViewModel AdminDashboard(string email);
        public AdminDashboardViewModel ProviderDashboard(string aspnetuserid);
        public AdminDashboardViewModel ProviderActiveTable(DashboardFilter filter, int physicianid);
        public AdminDashboardViewModel ProviderNewTable(DashboardFilter filter, int physicianid);
        public AdminDashboardViewModel ProviderConcludeTable(DashboardFilter filter, int physicianid);
        public AdminDashboardViewModel ProviderPendingTable(DashboardFilter filter, int physicianid);



    }
}
