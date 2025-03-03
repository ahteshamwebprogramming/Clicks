using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("ComplaintAttachmentFile")]
public class ComplaintAttachmentFile
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Id { get; set; }
    public Int64 ComplaintId { get; set; }

    public string? FilePath { get; set; } = null!;
    public string? ContentType { get; set; } = null!;
    public string? FileName { get; set; } = null!;
    public string? Length { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }    

    public bool IsActive { get; set; }

  
}
