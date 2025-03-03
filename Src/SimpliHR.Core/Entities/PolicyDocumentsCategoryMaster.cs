using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities
{
    public partial class PolicyDocumentsCategoryMaster
    {
        public int PolicyDocumentsCategoryId { get; set; }

        public string PolicyDocumentsCategory { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int? UnitId { get; set; }
        public virtual ICollection<PolicyDocumentsMaster> PolicyDocumentsMasters { get; set; } = new List<PolicyDocumentsMaster>();
        public virtual ICollection<PolicyDocumentsSubCategoryMaster> PolicyDocumentsSubCategoryMasters { get; set; } = new List<PolicyDocumentsSubCategoryMaster>();
    }
}
