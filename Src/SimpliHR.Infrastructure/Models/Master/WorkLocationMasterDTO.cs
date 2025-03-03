using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Infrastructure.Models.Masters;

public partial class WorkLocationMasterDTO
{
    public int WorkLocationId { get; set; }

    public string EncryptedId { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter location"),MaxLength(50, ErrorMessage = "Location cannot exceed 50 characters")]
    public string? Location { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter address")]
    public string? Address { get; set; }
    [DataType(DataType.Text)]
    
    [Range(1, int.MaxValue, ErrorMessage = "Please select country")]

    public int? UnitId { get; set; }
    public int? CountryId { get; set; }
  
    public string? CountryName { get; set; }
    [DataType(DataType.Text)]

    [Range(1, int.MaxValue, ErrorMessage = "Please select state")]
    public int? StateId { get; set; }
   
    public string? StateName { get; set; }
    [ValidateNever]
    public int? DistrictId { get; set; }
    [DataType(DataType.Text)]
    [Range(1, int.MaxValue, ErrorMessage = "Please select city")]

    public int? CityId { get; set; }
    [ValidateNever]
    public string? CityName { get; set; }
    
    [Required(ErrorMessage = "Please enter pin code")]
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

    public HttpResponseMessage? HttpMessage { get; set; }
    [ValidateNever]
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<WorkLocationMasterDTO>? WorkLocationMasterList { get; set; }
    [ValidateNever]
    public List<CountryKeyValues> CountryList { get; set; }
    [ValidateNever]
    public List<StateKeyValues> StateList { get; set; }
    [ValidateNever]
    public List<CityKeyValues> CityList { get; set; }
    [ValidateNever]
    public CountryMasterDTO Country { get; set; } = null!;
    [ValidateNever]
    public StateMasterDTO State { get; set; } = null!;
    [ValidateNever]
    public CityMasterDTO City { get; set; } = null!;

    public int? HttpStatusCode { get; set; } = 200;
}
