//using Dapper.Contrib.Extensions;
//using SimpliHR.Infrastructure.Models.KeyValue;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class SalaryTemplateKeyValues
{
    //[Key] // not [ExplicitKey]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SalaryTemplateId { get; set; }
    public string? TemplateName { get; set; }
    public int? UnitId { get; set; }
    

}
