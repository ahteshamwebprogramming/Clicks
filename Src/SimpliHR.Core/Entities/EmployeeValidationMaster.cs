using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("EmployeeValidationMaster")]
public partial class EmployeeValidationMaster
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int EmployeeValidationId { get; set; }
    public int? ClientId { get; set; }
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
    public string? TableName { get; set; }
}
