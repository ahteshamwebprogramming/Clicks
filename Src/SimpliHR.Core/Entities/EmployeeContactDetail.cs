using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeContactDetail
{
    public int EmployeeContactDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Landmark { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public int? CityId { get; set; }

    public string? Pincode { get; set; }

    public string? ContactNum { get; set; }

    public string? ContactType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }

}
