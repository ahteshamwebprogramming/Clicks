using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("ProfileEditAuth")]
public class ProfileEditAuth
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProfileEditAuthId { get; set; }

    public string? ProfileFieldName { get; set; }

    public int? ProfileFieldId { get; set; }
    public int? UnitId { get; set; }

    public string? ProfileFieldDisplayName { get; set; }

    public bool? IsEditable { get; set; }

    public bool? AttachmentRequired { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
}
