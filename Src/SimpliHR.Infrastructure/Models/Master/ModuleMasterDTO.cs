using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;

public partial class ModuleMasterDTO
{
    //[ScaffoldColumn(false)]

    public int ModuleId { get; set; }
    public string EncryptedId { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Module Short Name")]
    [Required(ErrorMessage = "Please enter Module Short Name"), MaxLength(100,ErrorMessage = "Module Short Name cannot exceed 100 characters")]
    public string? ModuleShortName { get; set; }
    [DataType(DataType.Text)]
    [Display(Name = "Module Name")]
    [Required(ErrorMessage = "Please enter Module Name"), MaxLength(200, ErrorMessage = "Module Name cannot exceed 200 characters")]
    public string ModuleName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<ModuleMasterDTO>? ModuleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;

}
