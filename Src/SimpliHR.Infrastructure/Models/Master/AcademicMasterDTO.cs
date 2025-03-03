using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class AcademicMasterDTO
{
    public int AcademicId { get; set; }
    public string EncryptedAcademicId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter academic code"), MaxLength(10, ErrorMessage = "Academic Code cannot exceed 10 characters")]
    public string? AcademicCode { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter academic"), MaxLength(100, ErrorMessage = "Academic Name cannot exceed 100 characters")]
    public string? AcademicName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<AcademicMasterDTO>? AcademicMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }

}
