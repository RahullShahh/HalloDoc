using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("healthprofessionaltype")]
public partial class Healthprofessionaltype
{
    [Key]
    [Column("healthprofessionalid")]
    public int Healthprofessionalid { get; set; }

    [Column("professionname")]
    [StringLength(50)]
    public string Professionname { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [InverseProperty("ProfessionNavigation")]
    public virtual ICollection<Healthprofessional> Healthprofessionals { get; set; } = new List<Healthprofessional>();
}
