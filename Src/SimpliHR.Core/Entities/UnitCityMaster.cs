using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace SimpliHR.Core.Entities;

[Table("UnitCityMaster")]
public partial class UnitCityMaster
{
    [Key]
    public int CityId { get; set; }

    public int CityParentId { get; set; }
    public int CountryId { get; set; }

    public int StateId { get; set; }

    [StringLength(50)]
    public string CityName { get; set; } = null!;

    public bool? IsActive { get; set; }

   
    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

  
    public int? ModifiedBy { get; set; }

    [Column("modifiedOn", TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public int UnitId { get; set; }

     [NavigationProperty]
    public virtual CountryMaster Country { get; set; } = null!;

    //   [ForeignKey("StateId")]
    //   [InverseProperty("CityMasters")]
    [NavigationProperty]
    public virtual UnitStateMaster State { get; set; } = null!;
}
