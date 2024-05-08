using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("shiftdetailregion")]
public partial class Shiftdetailregion
{
    [Key]
    [Column("shiftdetailregionid")]
    public int Shiftdetailregionid { get; set; }

    [Column("shiftdetailid")]
    public int Shiftdetailid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [ForeignKey("Regionid")]
    [InverseProperty("Shiftdetailregions")]
    public virtual Region Region { get; set; } = null!;

    [ForeignKey("Shiftdetailid")]
    [InverseProperty("Shiftdetailregions")]
    public virtual Shiftdetail Shiftdetail { get; set; } = null!;
}
