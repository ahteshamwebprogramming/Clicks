using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.WebUI.Modals.Masters
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }= DateTime.Now;
        public string? ModifedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
