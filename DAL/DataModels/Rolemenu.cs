using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("rolemenu")]
public partial class Rolemenu
{
    [Key]
    [Column("rolemenuid")]
    public int Rolemenuid { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [Column("menuid")]
    public int? Menuid { get; set; }

    [ForeignKey("Menuid")]
    [InverseProperty("Rolemenus")]
    public virtual Menu? Menu { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("Rolemenus")]
    public virtual Role? Role { get; set; }
}
