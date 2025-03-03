
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Masters;

public partial class PolicyDocumentsCategoryMasterDTO
{
    public int PolicyDocumentsCategoryId { get; set; }
    public string EncryptedId { get; set; }
    public string PolicyDocumentsCategory { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }
    public int? UnitId { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<PolicyDocumentsSubCategoryMasterDTO> PolicyDocumentsSubCategoryMasters { get; set; } = new List<PolicyDocumentsSubCategoryMasterDTO>();
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<PolicyDocumentsCategoryMasterDTO>? PolicyDocumentsCategoryMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;

}