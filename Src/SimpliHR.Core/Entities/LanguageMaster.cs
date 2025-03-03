using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("LanguageMaster")]
public partial class LanguageMaster
{
    [Key]
    public int LanguageId { get; set; }
    public string? Language { get; set; }
    public bool? IsActive { get; set; }
}
