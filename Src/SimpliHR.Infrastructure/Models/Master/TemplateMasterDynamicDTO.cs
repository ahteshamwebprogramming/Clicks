using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Master;

public class TemplateMasterDynamicDTO
{
    public int TemplateMasterDynamicId { get; set; }

    public string? FormName { get; set; }

    public string? Component { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public string? FormType { get; set; }
}
