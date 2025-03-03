using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PTStatus")]

public class PTStatus
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StatusID { get; set; }

    public string StatusName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? StatusType { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    [DefaultValue(true)]
    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
}
