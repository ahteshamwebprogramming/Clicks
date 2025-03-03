using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public class PageControlKeyValue
{
    public int KeyValueId { get; set; }
    public string KeyName { get; set; }
    public string KeyValue { get; set; }
    public string Module { get; set; }
    public string PageName { get; set; }
    public string ControlName { get; set; }

}
