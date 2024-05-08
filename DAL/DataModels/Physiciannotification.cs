using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("physiciannotification")]
public partial class Physiciannotification
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("isnotificationstopped")]
    public bool Isnotificationstopped { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Physiciannotifications")]
    public virtual Physician Physician { get; set; } = null!;
}
