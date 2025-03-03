using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Masters;

public partial class PolicyDocumentsSubCategoryMasterDTO
{
    public int PolicyDocumentsSubCategoryId { get; set; }
    public string EncryptedId { get; set; }
    public int? PolicyDocumentsCategoryId { get; set; }

    public string PolicyDocumentsSubCategory { get; set; } = null!;
    public string PolicyDocumentsCategoryName { get; set; } = null!;

    public bool IsActive { get; set; }
    public int? UnitId { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public List<PolicyDocumentsSubCategoryMasterDTO>? PolicyDocumentsSubCategoryMasterList { get; set; }

    public virtual PolicyDocumentsCategoryMasterDTO? PolicyDocumentsCategory { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<PolicyDocumentCategoryKeyValues>? PolicyDocumentCategoryList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
}
