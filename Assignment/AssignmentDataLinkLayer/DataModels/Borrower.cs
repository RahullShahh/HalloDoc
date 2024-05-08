using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AssignmentDataLinkLayer.DataModels;

[Table("Borrower")]
public partial class Borrower
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string? City { get; set; }

    [Column(TypeName = "character varying")]
    public string Name { get; set; } = null!;
}
