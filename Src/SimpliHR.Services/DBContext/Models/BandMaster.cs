using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("BandMaster")]
public partial class BandMaster
{
    [Key]
    [Column("BandID")]
    public int BandId { get; set; }

    [StringLength(10)]
    public string? BandCode { get; set; }

    [StringLength(50)]
    public string? Band { get; set; }

    [StringLength(500)]
    public string? BandDesc { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}
