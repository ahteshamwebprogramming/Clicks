using SimpliHR.Infrastructure.Models.ClientManagement;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class LanguageUnitMasterDTO
{
    public int LanguageId { get; set; }

    public string? Language { get; set; }

    public string? EncryptedId { get; set; }

    public int? LanguageParentId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<LanguageUnitMasterDTO>? LanguageUnitMasterList { get; set; }
    public LanguageUnitMasterDTO? LanguageMaster { get; set; }
    public int? HttpStatusCode { get; set; } = 200;


   

}
public partial class UnitLanguageListVM
{
    public int? HttpStatusCode { get; set; } = 200;

    public List<UnitMasterDTO>? Units { get; set; } = new List<UnitMasterDTO>();
    public LanguageUnitMasterDTO? UnitLanguage { get; set; } = new LanguageUnitMasterDTO();
    public List<LanguageUnitMasterDTO>? UnitLanguageList { get; set; } = new List<LanguageUnitMasterDTO>();
    public List<LanguageMasterDTO>? LanguageMasterList { get; set; } = new List<LanguageMasterDTO>();
    public string DisplayMessage { get; set; } = "_blank";
}