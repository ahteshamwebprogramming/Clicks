using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class PolicyDocumentKeyValues
{
    public int PolicyDocumentsMasterId { get; set; }
    public string PolicyDocument { get; set; } = null!;
    
}
