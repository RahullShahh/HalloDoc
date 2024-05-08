using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AssignmentDataLinkLayer.DataModels;

[Table("Book")]
public partial class Book
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string BookName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? Author { get; set; }

    public int? BorrowerId { get; set; }

    [Column(TypeName = "character varying")]
    public string? BorrowerName { get; set; }

    [Column(TypeName = "character varying")]
    public string? City { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? DateOfIssue { get; set; }

    public int? Genre { get; set; }
}
