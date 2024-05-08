namespace HalloDoc_Project.Extensions
{
    public static class Enumerations
    {
        public enum RequestStatus
        {
            Unassigned = 1,
            Accepted = 2,
            Cancelled = 3,
            MDEnRoute = 4,
            MDOnSite = 5,
            Conclude = 6,
            CancelledByPatient = 7,
            Closed = 8,
            Unpaid = 9,
            Clear = 10,
            Block = 11,
        }

        public enum AllowMenu
        {
            AdminDashboard = 1,
            ProviderLocation = 2,
            AdminProfile = 3,
            ProviderMenu = 4,
            Scheduling = 5,
            Invoicing = 6,
            Partners = 7,
            AccountAccess = 8,
            UserAccess = 9,
            SearchRecords = 10,
            EmailLogs = 11,
            SMSLogs = 12,
            PatientRecords = 13,
            BlockedHistory = 14,
            ProviderDashboard = 15,
            ProviderInvoicing = 16,
            ProviderSchedule = 17,
            ProviderProfile = 18,
        }

        public enum RequestCallType
        {
            HouseCall = 1,
            Consult = 2,
        }

        public enum DashboardStatus
        {
            New = 1,
            Pending = 2,
            Active = 3,
            Conclude = 4,
            ToClose = 5,
            Unpaid = 6,
        }

        public enum RequestType
        {
            Business = 1,
            Patient = 2,
            Family = 3,
            Concierge = 4
        }

        public enum AllowRole
        {
            Admin = 1,
            Patient = 2,
            Physician = 3
        }
    }
}

