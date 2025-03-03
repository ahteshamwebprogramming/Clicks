using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("TicketMaster")]
public class TicketMaster
{
    //[Dapper.Contrib.Extensions.Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [ExplicitKey]
    public string TicketId { get; set; }
    [ExplicitKey]
    public int? UnitId { get; set; }

    public int? ModuleId { get; set; }

    public string? TicketSource { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? Status { get; set; }

    public bool? IsActive { get; set; }
}
