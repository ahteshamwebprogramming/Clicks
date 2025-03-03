namespace SimpliHR.Core.Entities;

public partial class EmployeeUploadDocument
{
    public int UploadDcumentDetailId { get; set; }

    public int? EmployeeId { get; set; }
    public string? FieldName { get; set; }
    public string? ScreenTab { get; set; }
    public int? DcumentTypeId { get; set; }
    public string? DocumentType { get; set; }
    public byte[]? EmployeeDocument { get; set; }
   // public IFormFile UploadedFile { get; set; }
    public DateTime? CreatedOn { get; set; }

    public int? ReferenceId  { get; set; }
    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
}
