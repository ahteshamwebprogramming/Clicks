using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Infrastructure.Models.Masters;
public class UnitCityMasterDTO
{

    [Column("CityID")]
    public int CityId { get; set; }

    public int CityParentId { get; set; }
    public int UnitId { get; set; }
    public string EncryptedCityId { get; set; }

    [Column("CountryID")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select country")]
    public int CountryId { get; set; }
    public string? CountryName { get; set; }

    [Column("StateID")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select state")]
    public int StateId { get; set; }
    public string? StateName { get; set; }

    [StringLength(50)]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter city name"), MaxLength(50,ErrorMessage ="City cannot exceed 50 characters")]
    public string CityName { get; set; } = null!;

    public bool? IsActive { get; set; }

  
    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

  
    public int? ModifiedBy { get; set; }

    [Column("modifiedOn", TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    [ValidateNever]
    public CountryMasterDTO Country { get; set; } = null!;
    [ValidateNever]
    public UnitStateMasterDTO State { get; set; } = null!;

    //[ForeignKey("CountryId")]
    //[InverseProperty("CityMasters")]
    //   public virtual CountryMaster Country { get; set; } = null!;

    //[ForeignKey("StateId")]
    //[InverseProperty("CityMasters")]
    // public virtual StateMaster State { get; set; } = null!;
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    [ValidateNever]
    public List<UnitCityMasterDTO>? UnitCityMasterList { get; set; }

    public List<CountryKeyValues>? CountryList { get; set; }
    public List<StateKeyValues>? StateList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;


}

public partial class UnitCityListVM
{
    public int? HttpStatusCode { get; set; } = 200;

    public List<UnitMasterDTO>? Units { get; set; } = new List<UnitMasterDTO>();
    public UnitCityMasterDTO? UnitCity { get; set; } = new UnitCityMasterDTO();
    public List<UnitCityMasterDTO>? UnitCityList { get; set; } = new List<UnitCityMasterDTO>();
    public List<CityMasterDTO>? CityMasterList { get; set; } = new List<CityMasterDTO>();
    public string DisplayMessage { get; set; } = "_blank";
}



