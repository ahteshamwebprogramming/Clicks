using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class PageKeyValues
{
    public int KeyValueId { get; set; }
    public string KeyName { get; set; }
    public string KeyValue { get; set; }
    public string Module { get; set; }
    public string PageName { get; set; }
    public string ControlName { get; set; }

}
