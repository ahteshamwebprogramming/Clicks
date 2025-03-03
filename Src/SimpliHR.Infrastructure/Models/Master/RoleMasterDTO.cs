using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class RoleMasterDTO
{
    public int RoleId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter role"),MaxLength(100, ErrorMessage = "Role cannot exceed 100 characters")]
    public string? RoleName { get; set; }

    public string? RoleType { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }

}
