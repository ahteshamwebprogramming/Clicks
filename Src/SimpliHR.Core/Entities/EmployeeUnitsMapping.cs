using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Core.Entities;

[Table("EmployeeUnitsMapping")]
public partial class EmployeeUnitsMapping
    {
    [Key]
    public int EmployeeUnitID { get; set; }
    public int? ClientID { get; set; }
    public int? UnitID { get; set; }
    public int? EmployeeID { get; set; }
}

