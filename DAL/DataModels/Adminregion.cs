using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("adminregion")]
public partial class Adminregion
{
    [Key]
    [Column("adminregionid")]
    public int Adminregionid { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("Adminregions")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("Regionid")]
    [InverseProperty("Adminregions")]
    public virtual Region? Region { get; set; }
}
