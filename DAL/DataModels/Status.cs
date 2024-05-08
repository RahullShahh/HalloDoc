using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("status")]
public partial class Status
{
    [Key]
    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }
}
