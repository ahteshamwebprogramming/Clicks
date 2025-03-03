using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public class EditEmployeeDataDTO
{
    public int EmployeeUpdateId { get; set; }

    public int? EmployeeId { get; set; }
    
    public int? ClientId { get; set; }
    
    public string? ChangeType { get; set; }

    public string? ChangeValue { get; set; }

    public DateTime? Wefdate { get; set; }

    public string? OldValue { get; set; }

    public int? IsApproved { get; set; }

    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }

    public int? LoggedInUser { get; set; }
    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsActive { get; set; }

    [MaxLength]
    public byte[]? Attachment { get; set; }

    public IFormFile AttachmentFile { get; set; }
    public string? DocumentType { get; set; }
    public string Base64Attachment { get; set; }
    public int? EmployeeValidationId { get; set; }
    public int? TableReferenceId { get; set; }
    public string? FormName { get; set; }
    public string? DisplayMessage { get; set; } = "_blank";
}

public class EditEmployeeDataVM
{
    public string TicketId { get; set; }
    public string ScreenTab { get; set; }
    public EditEmployeeDataDTO EditEmployeeData { get; set; }
    public List<EditEmployeeDataDTO> EditEmployeeDataList { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
    public string ServerMessage { get; set; } = "_blank";
}
