using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class LeaveTypeKeyValues
{
    public int LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }
    public string? LeaveTypeCode { get; set; }
    public double? MaxLeaveRange { get; set; }

    public double? MinLeaveRange { get; set; }
}
