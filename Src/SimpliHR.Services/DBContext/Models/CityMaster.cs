using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("CityMaster")]
public partial class CityMaster
{
    [Key]
    [Column("CityID")]
    public int CityId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [Column("StateID")]
    public int StateId { get; set; }

    [StringLength(50)]
    public string CityName { get; set; } = null!;

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column("modifiedOn", TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("CityMasters")]
    public virtual CountryMaster Country { get; set; } = null!;

    [ForeignKey("StateId")]
    [InverseProperty("CityMasters")]
    public virtual StateMaster State { get; set; } = null!;
}
