using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("WorkLocationMaster")]
public partial class WorkLocationMaster
{
    [Key]
    [Column("WorkLocationID")]
    public int WorkLocationId { get; set; }

    [StringLength(50)]
    public string? Location { get; set; }

    public string? Address { get; set; }

    [Column("CountryID")]
    public int? CountryId { get; set; }

    [Column("StateID")]
    public int? StateId { get; set; }

    [Column("DistrictID")]
    public int? DistrictId { get; set; }

    [Column("CityID")]
    public int? CityId { get; set; }

    public long? Pincode { get; set; }

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
