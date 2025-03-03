using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class IdtypeMasterDTO
{
    public int IdentityId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter identity code"),MaxLength(10, ErrorMessage = "Identity Code cannot exceed 50 characters")]
    public string? IdentityCode { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter identity name"),MaxLength(100, ErrorMessage = "Identity cannot exceed 50 characters")]
    public string? IdentityName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<IdtypeMasterDTO>? IdtypeMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;

}
