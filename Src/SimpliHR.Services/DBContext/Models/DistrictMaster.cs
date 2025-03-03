using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("DistrictMaster")]
public partial class DistrictMaster
{
    [Key]
    [Column("DistrictID")]
    public int DistrictId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [Column("StateID")]
    public int StateId { get; set; }

    [Column("CityIDs")]
    [StringLength(50)]
    public string CityIds { get; set; } = null!;

    [StringLength(50)]
    public string? DistrictName { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("DistrictMasters")]
    public virtual CountryMaster Country { get; set; } = null!;

    [ForeignKey("StateId")]
    [InverseProperty("DistrictMasters")]
    public virtual StateMaster State { get; set; } = null!;
}
