using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Common;

public partial class AddDeleteTableActionDTO
{
    public int ActionId { get; set; }

    public int? ActionBy { get; set; }

    public int? ActionStatus { get; set; }

    public string? ActionType { get; set; }

    public string? TicketId { get; set; }

    public string? ReferenceTable { get; set; }

    public int? ReferenceId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EntrySource { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public int? LoggedInUser { get; set; }

    public string? DisplayMessage { get; set; } = "_blank";
    public string? FormName { get; set; } = "";
    public bool? IsActive { get; set; }
}


public partial class SchedularMessageDTO
{
    public string DisplayMessage { get; set; } = string.Empty;
}
