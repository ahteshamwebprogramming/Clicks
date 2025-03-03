using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class LeaveAttributeMasterKeyValues
{
    public ICollection<LeaveCalenderYearKeyValues> LeaveCalenderYearKeyValue { get; set; } = new List<LeaveCalenderYearKeyValues>();
    public ICollection<PolicyDocumentKeyValues> PolicyDocumentKeyValues { get; set; } = new List<PolicyDocumentKeyValues>();
    public ICollection<LeaveTypeKeyValues> LeaveTypeKeyValues { get; set; } = new List<LeaveTypeKeyValues>();
}
