using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("EmployeeFnFDetails")]
public partial class EmployeeFnFDetails
{
    [Key]
    public int EmployeeFnFId { get; set; }
    public int? EmployeeId { get; set; }

    public int? NoticePeriod { get; set; }

    public int? LeaveBalance { get; set; }

    public int? Gratuity { get; set; }

    public int? IsProcess { get; set; }

    public int? IsMailSent { get; set; }
    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
    public int? UnitId { get; set; }
    //public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();
}
