using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("EmployeesSalaryDetails")]
public partial class EmployeesSalaryDetails
{
     [Key]
    public int EmployeeSalaryId { get; set; }

    public int? EmployeeId { get; set; }

    public int? SalaryComponentId { get; set; }

    //[StringLength(50)]
    public string? SalaryComponentType { get; set; }

    public string? ComponentName { get; set; }

    //[StringLength(50)]
    public string? CalculationType { get; set; }

   // [StringLength(500)]
    public decimal? PerVal { get; set; }

    //[StringLength(500)]
    public decimal? AmtPerMonth { get; set; }

    //[StringLength(500)]
    public decimal? AmtPerYear { get; set; }

   // [StringLength(5)]
    public int? SalaryMonth { get; set; }

   // [StringLength(50)]
    public int? SalaryYear { get; set; }

    //[StringLength(50)]
    public bool IsSent { get; set; }
   
    public int? UnitId { get; set; }
    public DateTime? Processdate { get; set; }

    public string? TemplateType { get; set; }
    public string? ProcessBy { get; set; }
    public string? WEF { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsVisibleInPaySlip { get; set; }
    public bool? IsTaxableIncome { get; set; }
    public bool? IsEpfConsidration { get; set; }
    public bool? IsEsiConsidrable { get; set; }
    public bool IsVariable { get; set; }
    public bool IsArrears { get; set; }
    public decimal? ArrearsAmt { get; set; }
}
