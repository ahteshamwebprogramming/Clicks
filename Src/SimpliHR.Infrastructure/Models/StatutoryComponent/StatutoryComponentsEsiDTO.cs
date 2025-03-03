using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.StatutoryComponent;

public class StatutoryComponentsEsiDTO
{
    public int StatutoryComponentsEsiid { get; set; }
    public string StatutoryComponentsEsiIdEnc { get; set; }

    public int? UnitId { get; set; }

    public string? Esinumber { get; set; }

    public string? DeductionCycle { get; set; }

    public double? EmployeesContribution { get; set; }

    public double? EmployersContribution { get; set; }

    public bool IsEmployersContibutionInCtc { get; set; }

    public double? Esilimit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }
}
