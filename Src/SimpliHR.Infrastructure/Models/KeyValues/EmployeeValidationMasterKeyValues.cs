using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class EmployeeValidationKeyValues
{
    public int EmployeeValidationId { get; set; }
    public string? FieldName { get; set; }
    public string? DisplayText { get; set; }
    public string? ScreenName { get; set; }
    public string? ScreenTab { get; set; }
    public int? TabSequence { get; set; }
    public bool? AddAttachment { get; set; }
    public bool? EditAttachment { get; set; }
    public bool? IsMandatory { get; set; }

}
