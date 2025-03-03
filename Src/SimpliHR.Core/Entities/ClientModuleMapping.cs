using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class ClientModuleMapping
{
    public int ClientModuleMappingId { get; set; }

    public int ClientId { get; set; }

    public int ModuleId { get; set; }

    public virtual Client Client { get; set; } = null!;
}
