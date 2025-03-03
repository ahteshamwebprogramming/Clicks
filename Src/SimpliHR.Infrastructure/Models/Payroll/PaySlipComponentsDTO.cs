using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class PaySlipComponentsDTO
{
    public int ID { get; set; }

    public string? Types { get; set; }

    public float? PaidDays { get; set; }

    public bool? IsVariable { get; set; }

    public string? Component { get; set; }

    public decimal? Standard { get; set; }

    public decimal? Actual { get; set; }

    public decimal? Arrears { get; set; }

    public decimal? Total { get; set; }

    public string? DComponent { get; set; }

    public decimal? DActual { get; set; }

    public decimal? DArrears { get; set; }

    public decimal? DTotal { get; set; }


    public string DisplayMessage { get; set; } = "_blank";
}

