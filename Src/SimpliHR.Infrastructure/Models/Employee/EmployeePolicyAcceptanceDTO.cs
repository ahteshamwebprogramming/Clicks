using SimpliHR.Infrastructure.Models.ClientManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeePolicyAcceptanceDTO
{
    public int EmployeePolicyAcceptanceId { get; set; }

    public int PolicyDocumentsMasterId { get; set; }

    public string PolicyDocumentsCategory { get; set; }
    public string PolicyDocumentsSubCategory { get; set; }
    public string PolicyDocument { get; set; }
    public string PolicyDocumentPath { get; set; }
    public bool AcceptanceRequired { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public bool? Accepted { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual EmployeeMasterDTO Employee { get; set; } = null!;

    public virtual PolicyDocumentsMasterDTO PolicyDocumentsMaster { get; set; } = null!;
}
