using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class StatutoryComponentsLabourWelfareFund
{
    public int StatutoryComponentsLabourWelfareFundId { get; set; }

    public int UnitId { get; set; }

    public double? EmployeesContribution { get; set; }

    public double? EmployersContribution { get; set; }

    public string? DeductionCycle { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
}
