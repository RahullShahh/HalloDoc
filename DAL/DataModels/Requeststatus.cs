using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("requeststatus")]
public partial class Requeststatus
{
    [Key]
    [Column("statusid")]
    public int Statusid { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;
}
