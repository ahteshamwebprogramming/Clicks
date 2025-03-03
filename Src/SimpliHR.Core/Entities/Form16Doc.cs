using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class Fom16Doc
{
    public int FormId { get; set; }

    public string? DocName { get; set; }

    public byte[]? DocAttachment { get; set; }

    public string? Pannumber { get; set; }

    public int? EmployeeId { get; set; }

    public string? FinYear { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
