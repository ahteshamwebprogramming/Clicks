using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Infrastructure.Models.Masters;

public partial class LeaveTypeMasterDTO
{
    public int LeaveTypeId { get; set; }
    public string EncryptedId { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter leave code")]
    public string? LeaveTypeCode { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter leave name"), MaxLength(5, ErrorMessage = "Leave Type Code cannot exceed 5 characters")]
    public string? LeaveType { get; set; }
    //[DataType(DataType.Text)]
    //[Required(ErrorMessage = "Please enter max limit"), MaxLength(50, ErrorMessage = "Leave cannot exceed 50 characters")]
    public int? MaxLimit { get; set; }

    public double? MaxLeaveRange { get; set; }

    public double? MinLeaveRange { get; set; }
    public bool? IsActive { get; set; }

    public int? UnitId { get; set; }

    public string? ApplicableFor { get; set; }

    public bool? IsPaidLeave { get; set; }
    public bool? IsCompOff { get; set; }
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<LeaveTypeMasterDTO>? LeaveTypeMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
}
