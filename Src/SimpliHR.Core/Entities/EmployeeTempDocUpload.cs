using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

[Dapper.Contrib.Extensions.Table("EmployeeTempDocUpload")]
public partial class EmployeeTempDocUpload
{
    
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //public int UploadDcumentDetailId { get; set; }

    public int? EmployeeId { get; set; }
    [ExplicitKey]
    public string? SessionId { get; set; }
    
    [ExplicitKey]
    public string? FieldName { get; set; }
    public string? ScreenTab { get; set; }
    public int? CreatedBy { get; set; }
    public string? ActionStatus { get; set; }
    public int? ActionBy { get; set; }
    public DateTime? CreatedOn { get; set; }
   
    public int? ReferenceId { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public byte[]? UploadedFile { get; set; }

    public string? DocumentType { get; set; }
    //public virtual EmployeeMaster? Employee { get; set; }
}
