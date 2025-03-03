using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class ShiftKeyValues
{
    public int ShiftId { get; set; }
    public int UnitId { get; set; }
    public string? ShiftCode { get; set; }
    public string? ShiftName { get; set; }

}
