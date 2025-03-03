
namespace SimpliHR.Core.Entities;

public partial class EmployeeValidationMasterDTO
{

    public int EmployeeValidationId { get; set; }
    public int? ClientId { get; set; }

    public string? TableIdColumn { get; set; }

    public string? TableName { get; set; }
    public string? ScreenName { get; set; }

    public string? ScreenTab { get; set; }

    public int? TabSequence { get; set; }    

    public string? FieldName { get; set; }

    public string? DisplayText { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public string? TableType { get; set; }
  
    public string? DisplayMessage { get; set; } = "_";
}
