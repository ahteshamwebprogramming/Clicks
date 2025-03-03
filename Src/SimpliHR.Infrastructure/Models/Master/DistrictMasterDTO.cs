using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Infrastructure.Models.Masters;

public class DistrictMasterDTO
{
    public int DistrictId { get; set; }
    public int CountryId { get; set; }
    public int StateId { get; set; }
    public string StateName { get; set; }
    //[StringLength(50)]
    //public string CityIds { get; set; } = null!;

    [StringLength(50)]
    public string? DistrictName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<DistrictMasterDTO>? DistrictMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;

    public List<StateKeyValues>? StateList { get; set; }
    

    // public virtual CountryMaster Country { get; set; } = null!;
    //public virtual StateMaster State { get; set; } = null!;
}

