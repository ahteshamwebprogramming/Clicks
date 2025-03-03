using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.WebUI.Masters
{
    public class JobTitleVM
    {
        public int JobTitleId { get; set; }
        public string? JobTitle { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
