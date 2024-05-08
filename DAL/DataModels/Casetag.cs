using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Keyless]
[Table("casetag")]
public partial class Casetag
{
    [Column("casetagid")]
    public int? Casetagid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }
}
