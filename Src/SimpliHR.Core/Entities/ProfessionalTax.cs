using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("ProfessionalTax")]
public partial class ProfessionalTax
{
    [Key]
    public int ProfTaxId { get; set; }

    public decimal? ProfTax { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    public DateTime? WEFDate { get; set; }
    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }
    public string? Gender { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    //public virtual EmployeeMaster? Employee { get; set; }
    public int? UnitId { get; set; }

    [NavigationProperty]
    public virtual CountryMaster Country { get; set; } = null!;

    //   [ForeignKey("StateId")]
    //   [InverseProperty("CityMasters")]
    [NavigationProperty]
    public virtual StateMaster State { get; set; } = null!;
}
