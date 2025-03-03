using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class AcademicDetail
{
    public Guid AcademicDetailId { get; set; }

    public Guid? EmployeeId { get; set; }

    public int? AcademicId { get; set; }

    public string? AcademicName { get; set; }

    public decimal? Percentage { get; set; }

    public string? Institute { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
}
