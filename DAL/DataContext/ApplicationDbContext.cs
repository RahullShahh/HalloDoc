using System;
using System.Collections.Generic;
using DAL.DataModels;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Adminregion> Adminregions { get; set; }

    public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }

    public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

    public virtual DbSet<Aspnetuserrole> Aspnetuserroles { get; set; }

    public virtual DbSet<Blockrequest> Blockrequests { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<Casetag> Casetags { get; set; }

    public virtual DbSet<Concierge> Concierges { get; set; }

    public virtual DbSet<Emaillog> Emaillogs { get; set; }

    public virtual DbSet<Encounterform> Encounterforms { get; set; }

    public virtual DbSet<Healthprofessional> Healthprofessionals { get; set; }

    public virtual DbSet<Healthprofessionaltype> Healthprofessionaltypes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<PayrateByProvider> PayrateByProviders { get; set; }

    public virtual DbSet<PayrateCategory> PayrateCategories { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<Physicianlocation> Physicianlocations { get; set; }

    public virtual DbSet<Physiciannotification> Physiciannotifications { get; set; }

    public virtual DbSet<Physicianregion> Physicianregions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Requestbusiness> Requestbusinesses { get; set; }

    public virtual DbSet<Requestclient> Requestclients { get; set; }

    public virtual DbSet<Requestclosed> Requestcloseds { get; set; }

    public virtual DbSet<Requestconcierge> Requestconcierges { get; set; }

    public virtual DbSet<Requestnote> Requestnotes { get; set; }

    public virtual DbSet<Requeststatus> Requeststatuses { get; set; }

    public virtual DbSet<Requeststatuslog> Requeststatuslogs { get; set; }

    public virtual DbSet<Requesttype> Requesttypes { get; set; }

    public virtual DbSet<Requestwisefile> Requestwisefiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolemenu> Rolemenus { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Shiftdetail> Shiftdetails { get; set; }

    public virtual DbSet<Shiftdetailregion> Shiftdetailregions { get; set; }

    public virtual DbSet<Smslog> Smslogs { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Timesheet> Timesheets { get; set; }

    public virtual DbSet<TimesheetDetail> TimesheetDetails { get; set; }

    public virtual DbSet<TimesheetDetailReimbursement> TimesheetDetailReimbursements { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=rahulshah;Server=localhost;Port=5432;Database=HalloDocDB;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Adminid).HasName("admin_pkey");

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.AdminAspnetusers).HasConstraintName("admin_aspnetuserid_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.AdminModifiedbyNavigations).HasConstraintName("admin_modifiedby_fkey");
        });

        modelBuilder.Entity<Adminregion>(entity =>
        {
            entity.HasKey(e => e.Adminregionid).HasName("adminregion_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Adminregions).HasConstraintName("adminregion_adminid_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Adminregions).HasConstraintName("adminregion_regionid_fkey");
        });

        modelBuilder.Entity<Aspnetrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aspnetroles_pkey");
        });

        modelBuilder.Entity<Aspnetuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aspnetusers_pkey");
        });

        modelBuilder.Entity<Aspnetuserrole>(entity =>
        {
            entity.HasKey(e => new { e.Userid, e.Name }).HasName("aspnetuserroles_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserroles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aspnetuserroles_userid_fkey");
        });

        modelBuilder.Entity<Blockrequest>(entity =>
        {
            entity.HasKey(e => e.Blockrequestid).HasName("blockrequests_pkey");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("business_pkey");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.BusinessCreatedbyNavigations).HasConstraintName("business_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.BusinessModifiedbyNavigations).HasConstraintName("business_modifiedby_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Businesses).HasConstraintName("business_regionid_fkey");
        });

        modelBuilder.Entity<Concierge>(entity =>
        {
            entity.HasKey(e => e.Conciergeid).HasName("concierge_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Concierges).HasConstraintName("concierge_regionid_fkey");
        });

        modelBuilder.Entity<Emaillog>(entity =>
        {
            entity.HasKey(e => e.Emaillogid).HasName("emaillog_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Emaillogs).HasConstraintName("emaillog_adminid_fkey");
        });

        modelBuilder.Entity<Encounterform>(entity =>
        {
            entity.HasKey(e => e.Encounterformid).HasName("encounterform_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Encounterforms).HasConstraintName("encounterform_adminid_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Encounterforms).HasConstraintName("encounterform_physicianid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Encounterforms).HasConstraintName("encounterform_requestid_fkey");
        });

        modelBuilder.Entity<Healthprofessional>(entity =>
        {
            entity.HasKey(e => e.Vendorid).HasName("healthprofessionals_pkey");

            entity.HasOne(d => d.ProfessionNavigation).WithMany(p => p.Healthprofessionals).HasConstraintName("healthprofessionals_profession_fkey");
        });

        modelBuilder.Entity<Healthprofessionaltype>(entity =>
        {
            entity.HasKey(e => e.Healthprofessionalid).HasName("healthprofessionaltype_pkey");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Menuid).HasName("menu_pkey");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderdetails_pkey");
        });

        modelBuilder.Entity<PayrateByProvider>(entity =>
        {
            entity.HasKey(e => e.PayrateId).HasName("PayrateByProvider_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PayrateByProviderCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_createdby");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PayrateByProviderModifiedByNavigations).HasConstraintName("fk_modifiedby");

            entity.HasOne(d => d.PayrateCategory).WithMany(p => p.PayrateByProviders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payratecategory");

            entity.HasOne(d => d.Physician).WithMany(p => p.PayrateByProviders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_provider");
        });

        modelBuilder.Entity<PayrateCategory>(entity =>
        {
            entity.HasKey(e => e.PayrateCategoryId).HasName("PayrateCategory_pkey");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.Physicianid).HasName("physician_pkey");

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.PhysicianAspnetusers).HasConstraintName("physician_aspnetuserid_fkey");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.PhysicianCreatedbyNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physician_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.PhysicianModifiedbyNavigations).HasConstraintName("physician_modifiedby_fkey");
        });

        modelBuilder.Entity<Physicianlocation>(entity =>
        {
            entity.HasKey(e => e.Locationid).HasName("physicianlocation_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Physicianlocations).HasConstraintName("physicianlocation_physicianid_fkey");
        });

        modelBuilder.Entity<Physiciannotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("physiciannotification_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Physiciannotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physiciannotification_physicianid_fkey");
        });

        modelBuilder.Entity<Physicianregion>(entity =>
        {
            entity.HasKey(e => e.Physicianregionid).HasName("physicianregion_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Physicianregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physicianregion_physicianid_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Physicianregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physicianregion_regionid_fkey");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Regionid).HasName("region_pkey");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("request_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requests).HasConstraintName("request_physicianid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("request_userid_fkey");
        });

        modelBuilder.Entity<Requestbusiness>(entity =>
        {
            entity.HasKey(e => e.Requestbusinessid).HasName("requestbusiness_pkey");

            entity.HasOne(d => d.Business).WithMany(p => p.Requestbusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestbusiness_businessid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestbusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestbusiness_requestid_fkey");
        });

        modelBuilder.Entity<Requestclient>(entity =>
        {
            entity.HasKey(e => e.Requestclientid).HasName("requestclient_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Requestclients).HasConstraintName("requestclient_regionid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestclients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestclient_requestid_fkey");
        });

        modelBuilder.Entity<Requestclosed>(entity =>
        {
            entity.HasKey(e => e.Requestclosedid).HasName("requestclosed_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestcloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestclosed_requestid_fkey");

            entity.HasOne(d => d.Requeststatuslog).WithMany(p => p.Requestcloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestclosed_requeststatuslogid_fkey");
        });

        modelBuilder.Entity<Requestconcierge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requestconcierge_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Concierge).WithMany(p => p.Requestconcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestconcierge_conciergeid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestconcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestconcierge_requestid_fkey");
        });

        modelBuilder.Entity<Requestnote>(entity =>
        {
            entity.HasKey(e => e.Requestnotesid).HasName("requestnotes_pkey");

            entity.Property(e => e.Requestnotesid).ValueGeneratedNever();

            entity.HasOne(d => d.Request).WithMany(p => p.Requestnotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestnotes_requestid_fkey");
        });

        modelBuilder.Entity<Requeststatus>(entity =>
        {
            entity.HasKey(e => e.Statusid).HasName("requeststatus_pkey");

            entity.Property(e => e.Statusid).ValueGeneratedNever();
        });

        modelBuilder.Entity<Requeststatuslog>(entity =>
        {
            entity.HasKey(e => e.Requeststatuslogid).HasName("requeststatuslog_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Requeststatuslogs).HasConstraintName("requeststatuslog_adminid_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequeststatuslogPhysicians).HasConstraintName("requeststatuslog_physicianid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requeststatuslogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requeststatuslog_requestid_fkey");

            entity.HasOne(d => d.Transtophysician).WithMany(p => p.RequeststatuslogTranstophysicians).HasConstraintName("requeststatuslog_transtophysicianid_fkey");
        });

        modelBuilder.Entity<Requesttype>(entity =>
        {
            entity.HasKey(e => e.Requesttypeid).HasName("requesttype_pkey");
        });

        modelBuilder.Entity<Requestwisefile>(entity =>
        {
            entity.HasKey(e => e.Requestwisefileid).HasName("requestwisefile_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Requestwisefiles).HasConstraintName("requestwisefile_adminid_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requestwisefiles).HasConstraintName("requestwisefile_physicianid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestwisefiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestwisefile_requestid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("Role_pkey");
        });

        modelBuilder.Entity<Rolemenu>(entity =>
        {
            entity.HasKey(e => e.Rolemenuid).HasName("rolemenu_pkey");

            entity.HasOne(d => d.Menu).WithMany(p => p.Rolemenus).HasConstraintName("rolemenu_menuid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Rolemenus).HasConstraintName("rolemenu_roleid_fkey");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Shiftid).HasName("shift_pkey");

            entity.Property(e => e.Shiftid).UseIdentityAlwaysColumn();
            entity.Property(e => e.Weekdays).IsFixedLength();

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shift_createdby_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shift_physicianid_fkey");
        });

        modelBuilder.Entity<Shiftdetail>(entity =>
        {
            entity.HasKey(e => e.Shiftdetailid).HasName("shiftdetail_pkey");

            entity.Property(e => e.Shiftdetailid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.Shiftdetails).HasConstraintName("shiftdetail_modifiedby_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Shiftdetails).HasConstraintName("shiftdetail_regionid_fkey");

            entity.HasOne(d => d.Shift).WithMany(p => p.Shiftdetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shiftdetail_shiftid_fkey");
        });

        modelBuilder.Entity<Shiftdetailregion>(entity =>
        {
            entity.HasKey(e => e.Shiftdetailregionid).HasName("shiftdetailregion_pkey");

            entity.Property(e => e.Shiftdetailregionid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Region).WithMany(p => p.Shiftdetailregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shiftdetailregion_regionid_fkey");

            entity.HasOne(d => d.Shiftdetail).WithMany(p => p.Shiftdetailregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shiftdetailregion_shiftdetailid_fkey");
        });

        modelBuilder.Entity<Smslog>(entity =>
        {
            entity.HasKey(e => e.Smslogid).HasName("smslog_pkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("status_pkey");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Timesheet>(entity =>
        {
            entity.HasKey(e => e.TimesheetId).HasName("Timesheet_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TimesheetCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_createdby");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TimesheetModifiedByNavigations).HasConstraintName("fk_modifiedby");

            entity.HasOne(d => d.Physician).WithMany(p => p.Timesheets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physician");
        });

        modelBuilder.Entity<TimesheetDetail>(entity =>
        {
            entity.HasKey(e => e.TimesheetDetailId).HasName("TimesheetDetail_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TimesheetDetailCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_createdby");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TimesheetDetailModifiedByNavigations).HasConstraintName("fk_modifiedby");

            entity.HasOne(d => d.Timesheet).WithMany(p => p.TimesheetDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_timesheet");
        });

        modelBuilder.Entity<TimesheetDetailReimbursement>(entity =>
        {
            entity.HasKey(e => e.TimesheetDetailReimbursementId).HasName("TimesheetDetailReimbursement_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TimesheetDetailReimbursementCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_createdby");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TimesheetDetailReimbursementModifiedByNavigations).HasConstraintName("fk_modifiedby");

            entity.HasOne(d => d.TimesheetDetail).WithMany(p => p.TimesheetDetailReimbursements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_timesheetdetail");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("User_pkey");

            entity.Property(e => e.Userid).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.Users).HasConstraintName("User_aspnetuserid_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Users).HasConstraintName("User_regionid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
