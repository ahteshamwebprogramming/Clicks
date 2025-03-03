using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeDirectoryCardDetails
{

    public string? DisplayName { get; set; }

    public string? ColumnValue { get; set; }    
    public byte[]? ProfileImage { get; set; }
    public string? Base64ProfileImage { get; set; }
    public int? IsOptional { get; set; }
}
