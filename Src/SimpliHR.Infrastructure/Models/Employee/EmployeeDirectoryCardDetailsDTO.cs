using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public class EmployeeDirectoryCardDetailsDTO
{
    public string? DisplayName { get; set; }

    public string? ColumnValue { get; set; }
    public byte[]? ProfileImage { get; set; }
    public string? Base64ProfileImage { get; set; }
    public int? IsOptional { get; set; }

    public List<EmployeeDirectoryCardDetailsDTO>? EmployeeCardDetails { get; set; }
}


