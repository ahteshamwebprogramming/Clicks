using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTCommentDTO
{
    public int CommentID { get; set; }
    public string? encCommentID { get; set; }

    public int TaskID { get; set; }

    public string? encProjectID { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public bool IsVisible { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }
    public int UserID { get; set; }

    public int? ModifiedBy { get; set; }
   
    public bool? IsActive { get; set; }
}
