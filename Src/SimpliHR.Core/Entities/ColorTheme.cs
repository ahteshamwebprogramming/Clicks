using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class ColorTheme
{
    public int ColorId { get; set; }

    public string? ColorTheme1 { get; set; }

    public bool? IsActive { get; set; }
}
