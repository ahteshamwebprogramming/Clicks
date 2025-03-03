using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("StateMaster")]
public partial class StateMaster
{
    [Key]
    [Column("StateID")]
    public int StateId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(100)]
    public string StateName { get; set; } = null!;

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    [InverseProperty("State")]
    public virtual ICollection<CityMaster> CityMasters { get; set; } = new List<CityMaster>();

    [ForeignKey("CountryId")]
    [InverseProperty("StateMasters")]
    public virtual CountryMaster Country { get; set; } = null!;

    [InverseProperty("State")]
    public virtual ICollection<DistrictMaster> DistrictMasters { get; set; } = new List<DistrictMaster>();
}
