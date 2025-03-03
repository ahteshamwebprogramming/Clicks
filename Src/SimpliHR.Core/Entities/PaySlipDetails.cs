using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;


public partial class PaySlipDetails
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

   

   
}
