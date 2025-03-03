using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Page;

public class Error
{
    public string? Heading { get; set; }
    public string? Message { get; set; }
    public string? ButtonMessage { get; set; }
    public string? ButtonURL { get; set; }
}
