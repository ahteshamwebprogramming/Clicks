using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class EmployeeSalaryTemplateDetailDTO
{
    public int? EmployeeSalaryTemplateId { get; set; }

    public int? EmployeeId { get; set; }

    public string EmployeeName { get; set; }

}
