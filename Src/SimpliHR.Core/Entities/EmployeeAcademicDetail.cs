using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeAcademicDetail

{
    public int AcademicDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public int? AcademicId { get; set; }

    public int? PassingYear { get; set; }

    public string? Percentage { get; set; }

    public string? InstituteName { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
}
