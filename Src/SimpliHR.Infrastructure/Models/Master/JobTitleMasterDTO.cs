using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class JobTitleMasterDTO
{
    public int JobTitleId { get; set; }
    public string EncryptedJobTitleId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter job title"), MaxLength(50,ErrorMessage = "Job Title cannot exceed 50 characters")]
    public string? JobTitle { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<JobTitleMasterDTO>? JobTitleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;

    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }


}
