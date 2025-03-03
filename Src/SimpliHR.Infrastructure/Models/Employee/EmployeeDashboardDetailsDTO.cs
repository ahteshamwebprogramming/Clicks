using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public class EmployeeDashboardDetailsDTO
{
    public string? EmployeeCode { get; set; }
    public int? Id { get; set; }

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


