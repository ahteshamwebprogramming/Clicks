using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class MaritalStatusMasterDTO
{
    public int MaritalStatusId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter marital status"), MaxLength(100, ErrorMessage = "Marital Status cannot exceed 100 characters")]
    public string? MaritalStatusName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<MaritalStatusMasterDTO>? MaritalStatusMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;

}
