using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("TimesheetDetailReimbursement")]
public partial class TimesheetDetailReimbursement
{
    [Key]
    public int TimesheetDetailReimbursementId { get; set; }

    public int TimesheetDetailId { get; set; }

    [StringLength(500)]
    public string ItemName { get; set; } = null!;

    public int Amount { get; set; }

    [StringLength(500)]
    public string Bill { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TimesheetDetailReimbursementCreatedByNavigations")]
    public virtual Aspnetuser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimesheetDetailReimbursementModifiedByNavigations")]
    public virtual Aspnetuser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimesheetDetailId")]
    [InverseProperty("TimesheetDetailReimbursements")]
    public virtual TimesheetDetail TimesheetDetail { get; set; } = null!;
}
