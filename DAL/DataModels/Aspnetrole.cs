using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("aspnetroles")]
public partial class Aspnetrole
{
    [Key]
    [Column("id")]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; } = null!;
}
