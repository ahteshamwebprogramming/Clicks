using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PTComment")]
public class PTComment
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CommentID { get; set; }

    public int TaskID { get; set; }
    

    public string CommentText { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public bool IsVisible { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }
    public int UserID { get; set; }

    public int? ModifiedBy { get; set; }
    [DefaultValue(true)]
    public bool? IsActive { get; set; }

    
}
