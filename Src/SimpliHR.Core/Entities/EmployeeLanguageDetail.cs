using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class EmployeeLanguageDetail
{
    public int EmployeeLanguageDetailId { get; set; }

    public int EmployeeId { get; set; }

    public int? LanguageId { get; set; }

    public bool? Read { get; set; }

    public bool? Write { get; set; }

    public bool? Speak { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public virtual EmployeeMaster? Employee { get; set; }
}
