using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValueModels;
public partial class ModuleKeyValues
{
    public int ModuleId { get; set; }
    public string? ModuleName { get; set; }
}
