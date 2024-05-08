using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("Role")]
public partial class Role
{
    [Key]
    [Column("roleid")]
    public int Roleid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("accounttype")]
    public short? Accounttype { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("isdeleted")]
    public bool Isdeleted { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<Rolemenu> Rolemenus { get; set; } = new List<Rolemenu>();
}
