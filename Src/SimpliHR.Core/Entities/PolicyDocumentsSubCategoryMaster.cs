using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities
{
    public partial class PolicyDocumentsSubCategoryMaster
    {
        public int PolicyDocumentsSubCategoryId { get; set; }
        public int? UnitId { get; set; }
        public int? PolicyDocumentsCategoryId { get; set; }

        public string PolicyDocumentsSubCategory { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [NavigationProperty]
        public virtual PolicyDocumentsCategoryMaster? PolicyDocumentsCategory { get; set; }

        public virtual ICollection<PolicyDocumentsMaster> PolicyDocumentsMasters { get; set; } = new List<PolicyDocumentsMaster>();
    }
}
