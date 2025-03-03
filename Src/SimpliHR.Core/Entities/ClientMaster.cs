using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("Client")]
public partial class ClientMaster
{


    [Key]
    public int ClientId { get; set; }

    public string? ClientName { get; set; }

    public string? CompanyName { get; set; }

    public string? Gstn { get; set; }

    public string? EmailId { get; set; }

    public long? ContactNumber { get; set; }

    public string? Address { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public int? CityId { get; set; }

    public long? Pincode { get; set; }

    public string? ClientLogo { get; set; }

    public string? HeaderText { get; set; }

    public string? FooterText { get; set; }

    public string? SupportLink { get; set; }

    public string? PoliciesLink { get; set; }

    public string? DocumentLink { get; set; }

    public string? MenuStyle { get; set; }

    public string? ColorTheme { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<ClientModuleMapping> ClientModuleMappings { get; set; } = new List<ClientModuleMapping>();

    [NavigationProperty]
    public virtual CountryMaster Country { get; set; } = null!;

    //   [ForeignKey("StateId")]
    //   [InverseProperty("CityMasters")]
    [NavigationProperty]
    public virtual StateMaster State { get; set; } = null!;

    [NavigationProperty]
    public virtual CityMaster City { get; set; } = null!;
}
