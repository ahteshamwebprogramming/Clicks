using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Infrastructure.Models.Masters;

public partial class EmployeeValidationDTO
{
    public int EmployeeValidationId { get; set; }

    public int? UnitId { get; set; }

    public int? ClientId { get; set; }

    public string? ScreenName { get; set; }

    public string? ScreenTab { get; set; }

    public int? TabSequence { get; set; }

    public string? TableName { get; set; }

    public string? FieldName { get; set; }

    public string? DisplayText { get; set; }

    public bool? AddAttachment { get; set; }

    public bool? EditAttachment { get; set; }

    public bool? IsMandatory { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public string? TableType { get; set; }
    
}
public partial class EmployeeValidationVM
{
    public int? UnitId { get; set; }

    public int? ClientId { get; set; }

    public int? LocationId { get; set; }

    public string? ScreenName { get; set; }

    public string? ScreenTab { get; set; }

    public int? LogInUser { get; set; }

    public List<EmployeeValidationKeyValues> ScreenTabList { get; set; }

    public EmployeeValidationDTO EmployeeValidation { get; set; }=new EmployeeValidationDTO();
    public List<EmployeeValidationDTO> EmployeeValidationList { get; set; }=new List<EmployeeValidationDTO>();

    public List<PageKeyValues> YesNoOptionList { get; set; } = new List<PageKeyValues>();
    public string? DisplayMessage { get; set; } = "_Blank";
    public string? PageAction { get; set; } = "List";
    
}