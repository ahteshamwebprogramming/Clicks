using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTProjectMemberWithChild
{
    public int ProjectMemberID { get; set; }

    public int ProjectID { get; set; }
    public string RoleType { get; set; }
    public int UserID { get; set; }
    public int? UnitId { get; set; }
    public string? EmployeeName { get; set; }
    public string? UserProfileImagePath { get; set; }
    public string? ProfileImageExtension { get; set; }
    public byte[]? ProfileImage { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
}
