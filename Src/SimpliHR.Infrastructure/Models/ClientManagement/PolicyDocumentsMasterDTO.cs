using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.Infrastructure.Models.ClientManagement;

public partial class PolicyDocumentsMasterDTO
{
    public int PolicyDocumentsMasterId { get; set; }

    [ValidateNever]
    public string EncryptedId { get; set; }
    public int PolicyDocumentsCategoryId { get; set; }

    public int PolicyDocumentsSubCategoryId { get; set; }
    public int ClientId { get; set; }
    public int UnitId { get; set; }

    [ValidateNever]
    public string PolicyDocument { get; set; } = null!;

    public IFormFile PolicyDocumentFile { get; set; } = null!;

    [ValidateNever]
    public string PolicyDocumentPath { get; set; } = null!;

    public string? Description { get; set; }

    public bool AcceptanceRequired { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [ValidateNever]
    public virtual PolicyDocumentsCategoryMasterDTO PolicyDocumentsCategory { get; set; } = null!;

    [ValidateNever]
    public virtual PolicyDocumentsSubCategoryMasterDTO PolicyDocumentsSubCategory { get; set; } = null!;

    public ICollection<PolicyDocumentCategoryKeyValues> PolicyDocumentCategoryKeyValues { get; set; } = new List<PolicyDocumentCategoryKeyValues>();

    public HttpResponseMessage? HttpMessage { get; set; }
    public string? DisplayMessage { get; set; } = "";
}
