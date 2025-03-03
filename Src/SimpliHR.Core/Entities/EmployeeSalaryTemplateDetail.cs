using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeSalaryTemplateDetail
{
    
    [ExplicitKey]
    public int EmployeeSalaryTemplateId { get; set; }

    [ExplicitKey]
    public int EmployeeId { get; set; }
}
