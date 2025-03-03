using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Masters;

public class ClientMasterDTO
{

    [Key]
    [Column("ClientID")]
    public int ClientId { get; set; }

    [StringLength(50)]
    public string? ClientName { get; set; }

    [StringLength(500)]
    public string? CompanyName { get; set; }

    [StringLength(500)]
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

    public bool? IsHeader { get; set; }

    public bool? IsThemeChanges { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }

}



