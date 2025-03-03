using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Infrastructure.Models.Masters;

public class UnitStateMasterDTO
{
    [ScaffoldColumn(false)]
    public int StateId { get; set; }

    public int StateParentId { get; set; }

    public int UnitId { get; set; }
    public string EncryptedStateId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select country")]
    public int CountryId { get; set; }
    [ValidateNever]
    public string CountryName { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter state"), MaxLength(100 , ErrorMessage ="State cannot exceed 100 characters")]
    public string StateName { get; set; } = null!;
    public bool? IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    [ValidateNever]
    public CountryMasterDTO Country { get; set; }
    [ValidateNever]
    public CountryMasterDTO CountryMaster { get; set; }
    public bool IsIncludes { get; set; } = false;
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<UnitStateMasterDTO>? UnitStateMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<CountryKeyValues>? CountryList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;


}


public partial class UnitStateListVM
{
    public int? HttpStatusCode { get; set; } = 200;

    public List<UnitMasterDTO>? Units { get; set; } = new List<UnitMasterDTO>();
    public UnitStateMasterDTO? UnitState { get; set; } = new UnitStateMasterDTO();
    public List<UnitStateMasterDTO>? UnitStateList { get; set; } = new List<UnitStateMasterDTO>();
    public List<StateMasterDTO>? StateMasterList { get; set; } = new List<StateMasterDTO>();
    public string DisplayMessage { get; set; } = "_blank";
}
