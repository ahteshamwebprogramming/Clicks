using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeDashboardDetails
{

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public string? JobTitle { get; set; }

    public DateTime? DOJ { get; set; }

    public string? Location { get; set; }

    public DateTime? DOB { get; set; }

    public string? Anniversary { get; set; }
    public byte[]? ProfileImage { get; set; }
    public string? Base64ProfileImage { get; set; }
    public int? IsCurrent { get; set; }
}
