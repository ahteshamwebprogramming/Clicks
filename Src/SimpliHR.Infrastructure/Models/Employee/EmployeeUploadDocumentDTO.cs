using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeUploadDocumentDTO
{
    public int UploadDcumentDetailId { get; set; }

    public int? EmployeeId { get; set; }
    public string? FieldName { get; set; }
    public string? ScreenTab { get; set; }
    public int? EMail { get; set; }
    public int? DcumentTypeId { get; set; }
    public string? DocumentType { get; set; }

    public byte[]? EmployeeDocument { get; set; }
    public string Base64Document { get; set; }

    //public IFormFile? UploadedFile { get; set; }

    public IFormFile? DocumentNameFile { get; set; }

    public int? ReferenceId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public string FormName { get; set; }
    public int? ClientId { get; set; }
    public EmployeeMasterDTO? Employee { get; set; }

    public string DisplayMessage { get; set; } = "_blank";
}
