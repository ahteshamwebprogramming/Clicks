using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class ReligionMasterDTO
{
    public int ReligionId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter religion"),MaxLength(100, ErrorMessage = "Religion cannot exceed 100 characters")]
    public string? ReligionName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<ReligionMasterDTO>? ReligionMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;


}
