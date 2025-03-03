using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public class ShiftMasterDTO
{
    public int ShiftId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter shift code"), MaxLength(5, ErrorMessage = "Shift Code cannot exceed 5 characters")]
    public string? ShiftCode { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter shift"), MaxLength(100, ErrorMessage = "Shift cannot exceed 100 characters")]
    public string? ShiftName { get; set; }
    [DataType(DataType.Time)]
    [Required(ErrorMessage = "Please enter shift in time")]
    public TimeSpan? InTime { get; set; }
    [DataType(DataType.Time)]
    [Required(ErrorMessage = "Please enter shift out time")]
    public TimeSpan? OutTime { get; set; }

    public bool? IsNightShift { get; set; }

    public bool? isBufferTimeAllowed { get; set; }
    public bool? IsFlexi { get; set; }
    public bool? IsActive { get; set; }
    public int? BufferOfInTime { get; set; }
    public int? BufferOfOutTime { get; set; }
    public int? NoOfLateAllowed { get; set; }
    public int? IncludeBefore { get; set; }
    public int? IncludeAfter { get; set; }
    public int? PolicyId { get; set; }

    public int? UnitId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<ShiftMasterDTO>? ShiftMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;

}
