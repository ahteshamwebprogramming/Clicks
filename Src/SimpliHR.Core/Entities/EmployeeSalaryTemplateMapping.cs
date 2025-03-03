using Dapper.Contrib.Extensions;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("EmployeeSalaryTemplateMapping")]

public partial class EmployeeSalaryTemplateMapping
{
    [Key] // not [ExplicitKey]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeSalaryTemplateId { get; set; }

    public string? MappingName { get; set; }

    public int? SalaryTemplateId { get; set; }

    public int DepartmentId { get; set; }

    public int? EmployeesSelection { get; set; }

    //public string MappingEmployeeIds { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public bool IsActive { get; set; }
}
