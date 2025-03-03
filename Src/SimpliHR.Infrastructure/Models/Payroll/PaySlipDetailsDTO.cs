using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class PaySlipDetailsDTO
{
    public int EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? EpfNumber { get; set; }

    public string? AccountNo { get; set; }

    public string? BankName { get; set; }

    public string? JobTitle { get; set; }

    public string? DepartmentName { get; set; }

    public string? UnitName { get; set; }

    public string? Address { get; set; }

    public string? StateName { get; set; }

    public string? CityName { get; set; }

   // public byte[]? ProfileImage { get; set; }
    //public string Base64ProfileImage { get; set; }

    public string DisplayMessage { get; set; } = "_blank";
}

