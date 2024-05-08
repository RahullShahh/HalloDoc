using BAL.Interfaces;
using BAL.Repository;
using System.Runtime.CompilerServices;
using BAL.Interfaces.InterfaceProviderLocation;
using BAL.Repository.ProviderLocationRepository;
using BAL.Interfaces.IExcelMethods;
using BAL.Repository.ExcelMethodsRepository;
using BAL.Interfaces.IAdminRecords;
using BAL.Repository.AdminRecordsRepo;
using System.Web.Razor.Generator;
using DAL.DataModels;
using BAL.Interfaces.IProvider;
using BAL.Repository.ProviderRepo;
using BAL.Interfaces.IAccessMethods;
using BAL.Repository.AccessMethodsRepo;

namespace HalloDoc_Project.Extensions
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestRepo, RequestRepo>();
            services.AddScoped<IJwtToken, JwtTokenServices>();
            services.AddScoped<IEmailService, EmailServiceRepo>();
            services.AddScoped<IResetPasswordService, ResetPasswordServiceRepo>();
            services.AddScoped<IPasswordHasher, PasswordHasherRepo>();
            services.AddScoped<IFileOperations, FileOperationsRepo>();
            services.AddScoped<ILogin, LoginRepo>();
            services.AddScoped<IAgreement, AgreementRepo>();
            services.AddScoped<IAdminTables, AdminTablesRepo>();
            services.AddScoped<IAdminActions, AdminActionsRepo>();
            services.AddScoped<IPatientDashboard, PatientDashboardRepo>();
            services.AddScoped<IEncounterForm, EncounterFormRepo>();
            services.AddScoped<IAdmin, AdminRepo>();
            services.AddScoped<IProviderLocation, ProviderLocationRepo>();
            services.AddScoped<IHelperMethodsRepo, HelperMethodsRepo>();
            services.AddScoped<IExcelExport, ExcelExportRepo>();
            services.AddScoped<IPatientHistoryPatientRecords,PatientHistoryPatientRecordsRepo>();
            services.AddScoped<ISearchRecords, SearchRecordsRepo>();
            services.AddScoped<IBlockHistory,BlockHistoryRepo>();
            services.AddScoped<IEmailSMSLogs, EmailSMSLogsRepo>();
            services.AddScoped<IVendorDetails, VendorDetailsRepo>();
            services.AddScoped<ICreateEditProviderRepo,CreateEditProviderRepo>();
            services.AddScoped<IUserAccountAccessMethods,UserAccountAccessMethodsRepo>();
            return services;
        }
    }
}
