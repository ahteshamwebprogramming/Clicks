using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProfileEditAuth;

public class EmployeeEditInfoViewModel
{
    public int? EmployeeUpdateId { get; set; }
    public int? EmployeeId { get; set; }
    public string? ChangeValue { get; set; }
    public int? EmployeeValidationId { get; set; }
    public string? OldValue { get; set; }
    public int? IsApproved { get; set; }
    public int? ApprovedBy { get; set; }
    public string? TicketId { get; set; }
    public string? EntrySource { get; set; }
    [MaxLength]
    public byte[]? Attachment { get; set; }
    public string? AttachmentBase64String { get; set; }

    public byte[]? OldProfile { get; set; }
    public string? OldProfileBase64String { get; set; }

    public string? DocumentType { get; set; }

    public string? ScreenName { get; set; }
    public string? ScreenTab { get; set; }
    public int? TabSequence { get; set; }
    public string? FieldName { get; set; }
    public string? DisplayText { get; set; }
    public string? FieldType { get; set; }
    public string? NewValueText { get; set; }
    public string? OldValueText { get; set; }
}
