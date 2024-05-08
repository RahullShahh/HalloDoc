using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("requestwisefile")]
public partial class Requestwisefile
{
    [Key]
    [Column("requestwisefileid")]
    public int Requestwisefileid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("filename")]
    [StringLength(500)]
    public string Filename { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("doctype")]
    public short? Doctype { get; set; }

    [Column("isfrontside")]
    public bool? Isfrontside { get; set; }

    [Column("iscompensation")]
    public bool? Iscompensation { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("isfinalize")]
    public bool? Isfinalize { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [Column("ispatinetrecords")]
    public bool? Ispatinetrecords { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("Requestwisefiles")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Requestwisefiles")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Requestwisefiles")]
    public virtual Request Request { get; set; } = null!;
}
