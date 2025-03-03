using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Master;

public class TicketMasterDTO
{
    public int TicketId { get; set; }

    public string? TicketCode { get; set; }

    public int? ModuleId { get; set; }

    public string? TicketSource { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? Status { get; set; }

    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
    public string? CreatedByName { get; set; }
}
