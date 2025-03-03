using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("WorkLocationMaster")]
public partial class WorkLocationMaster
{
    [Key]
    public int WorkLocationId { get; set; }

    [StringLength(50)]
    public string? Location { get; set; }

    public int? UnitId { get; set; }

    public string? Address { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public int? DistrictId { get; set; }

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
    
    [NavigationProperty]
    public virtual CountryMaster Country { get; set; } = null!;

    //   [ForeignKey("StateId")]
    //   [InverseProperty("CityMasters")]
    [NavigationProperty]
    public virtual StateMaster State { get; set; } = null!;
    
    [NavigationProperty]
    public virtual CityMaster City { get; set; } = null!;

}
