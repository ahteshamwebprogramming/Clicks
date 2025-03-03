using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValues;

public class NewsCategoryTagKeyValues
{
    public int NewsCategoryTagId { get; set; }
    public string? encNewsCategoryTagId { get; set; }

    public string NewsCategoryTag { get; set; } = null!;
}
