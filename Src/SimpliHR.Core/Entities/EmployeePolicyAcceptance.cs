using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public partial class EmployeePolicyAcceptance
{
    public int EmployeePolicyAcceptanceId { get; set; }

    public int PolicyDocumentsMasterId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public bool? Accepted { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    //[NavigationProperty]
    public virtual EmployeeMaster Employee { get; set; } = null!;
    [NavigationProperty]
    public virtual PolicyDocumentsMaster PolicyDocumentsMaster { get; set; } = null!;
}

