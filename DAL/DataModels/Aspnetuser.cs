using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("aspnetusers")]
[Index("Email", Name = "UniqueEmail", IsUnique = true)]
public partial class Aspnetuser
{
    [Key]
    [Column("id")]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [Column("username")]
    [StringLength(256)]
    public string Username { get; set; } = null!;

    [Column("passwordhash")]
    public string? Passwordhash { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("phonenumber")]
    public string? Phonenumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("role")]
    [StringLength(50)]
    public string? Role { get; set; }

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Admin> AdminAspnetusers { get; set; } = new List<Admin>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Admin> AdminModifiedbyNavigations { get; set; } = new List<Admin>();

    [InverseProperty("User")]
    public virtual ICollection<Aspnetuserrole> Aspnetuserroles { get; set; } = new List<Aspnetuserrole>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Business> BusinessCreatedbyNavigations { get; set; } = new List<Business>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Business> BusinessModifiedbyNavigations { get; set; } = new List<Business>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<PayrateByProvider> PayrateByProviderCreatedByNavigations { get; set; } = new List<PayrateByProvider>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<PayrateByProvider> PayrateByProviderModifiedByNavigations { get; set; } = new List<PayrateByProvider>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Physician> PhysicianAspnetusers { get; set; } = new List<Physician>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedbyNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedbyNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Shiftdetail> Shiftdetails { get; set; } = new List<Shiftdetail>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Timesheet> TimesheetCreatedByNavigations { get; set; } = new List<Timesheet>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TimesheetDetail> TimesheetDetailCreatedByNavigations { get; set; } = new List<TimesheetDetail>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TimesheetDetail> TimesheetDetailModifiedByNavigations { get; set; } = new List<TimesheetDetail>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TimesheetDetailReimbursement> TimesheetDetailReimbursementCreatedByNavigations { get; set; } = new List<TimesheetDetailReimbursement>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TimesheetDetailReimbursement> TimesheetDetailReimbursementModifiedByNavigations { get; set; } = new List<TimesheetDetailReimbursement>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Timesheet> TimesheetModifiedByNavigations { get; set; } = new List<Timesheet>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
