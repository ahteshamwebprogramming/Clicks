using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTCommentWithChild
{
    public int CommentID { get; set; }
    public string? encCommentID { get; set; }

    public int TaskID { get; set; }

    public int EmployeeId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public bool IsVisible { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }
    public int UserID { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public byte[]? ProfileImage { get; set; }
    public string? ProfileImagePath { get; set; }
    public string? Orien { get; set; }
    public bool? AttachmentFound { get; set; }
}
