using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public partial class PolicyDocumentsMaster
{
    public int PolicyDocumentsMasterId { get; set; }

    public int PolicyDocumentsCategoryId { get; set; }

    public int PolicyDocumentsSubCategoryId { get; set; }
    public int ClientId { get; set; }
    public int UnitId { get; set; }

    public string PolicyDocument { get; set; } = null!;

    public string PolicyDocumentPath { get; set; } = null!;

    public string? Description { get; set; }

    public bool AcceptanceRequired { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [NavigationProperty]
    public virtual PolicyDocumentsCategoryMaster PolicyDocumentsCategory { get; set; } = null!;

    [NavigationProperty]
    public virtual PolicyDocumentsSubCategoryMaster PolicyDocumentsSubCategory { get; set; } = null!;

    //[NavigationProperty]
    public virtual ICollection<EmployeePolicyAcceptance> EmployeePolicyAcceptances { get; set; } = new List<EmployeePolicyAcceptance>();
}
