using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("EmployeeValidation")]
public partial class EmployeeValidation
{
    [ExplicitKey]
    public int EmployeeValidationId { get; set; }   
    public int? ClientId { get; set; }
    [ExplicitKey]
    public int UnitId { get; set; }
    public string? ScreenName { get; set; }

    public string? ScreenTab { get; set; }

    public int? TabSequence { get; set; }

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
    public string? TableName { get; set; }
}
