using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeTempDocUploadDTO
{
    //public int UploadDcumentDetailId { get; set; }

    public int? EmployeeId { get; set; }
    public string? EmailId { get; set; }
    public string? SessionId { get; set; } 
    public int? ActionBy { get; set; }
    public string? ActionStatus { get; set; }

    //public byte[]? EmployeeDocument { get; set; }
    public string Base64Document { get; set; }
    public string? FieldName { get; set; }
    public string ChangeValue { get; set; }
    public string? ScreenTab { get; set; }  
    public byte[]? UploadedFile { get; set; }

    public IFormFile? DocumentNameFile { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? TicketId { get; set; }

    public string? TableReferenceId { get; set; }
    
    public int? ModifiedBy { get; set; }

    public int? LoggedInUser { get; set; }
    public bool? IsActive { get; set; }
    public string FormName { get; set; }
    public string? DocumentType { get; set; }
    public int? ClientId { get; set; }
    public int? ReferenceId { get; set; }
    public string EntrySource { get; set; } = "";
    public string DisplayMessage { get; set; } = "_blank";
    public EmployeeMasterDTO? Employee { get; set; }
}
