using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeEJoineeDTO
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Please enter employee first name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Please enter employee code")]
    public string? EmployeeCode { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    [ValidateNever]
    public string EmployeeName { get; set; } = null!;

    [DataType(DataType.Date, ErrorMessage = "Date is not in a correct format")]
    [Required(ErrorMessage = "Please enter Date of Birth")]
    [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
    public DateTime? Dob { get; set; }

    [Required(ErrorMessage = "Please enter Email Id")]
    [DataType(DataType.EmailAddress)]
    public string? EmailId { get; set; }

    [Required(ErrorMessage = "Please enter Contact No")]

    [MaxLength(15, ErrorMessage = "Contact No cannot exceed 15 digits")]

    [MinLength(10, ErrorMessage = "Contact No must be at least 10 digits")]
    public string? ContactNo { get; set; }

    [DataType(DataType.Text)]
    [Range(1, int.MaxValue, ErrorMessage = "Please select gender")]
    public int? GenderId { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Date is not in a correct format")]
    [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
    [Required(ErrorMessage = "Please enter Date of Joining")]
    public DateTime? Doj { get; set; }

    [DataType(DataType.Text)]
    [Range(1, int.MaxValue, ErrorMessage = "Please select department")]
    public int? DepartmentId { get; set; }

    public string? DepartmentName { get; set; }
    public string? JobTitle { get; set; }

    public int? JoinType { get; set; }

    [DataType(DataType.Text)]
    [Range(1, int.MaxValue, ErrorMessage = "Please select designation")]
    public int? JobTitleId { get; set; }
    public DateTime? CreatedOn { get; set; }

    public decimal? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public decimal? ModifiedBy { get; set; }
    public bool? IsActive { get; set; }
    public int? EmailProvider { get; set; }

    [ValidateNever]
    public EmployeeMastersKeyValues? EmployeeMastersKeyValues { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public string? DisplayMessage { get; set; }

    public int? ClientId { get; set; }
    public int? UnitId { get; set; }

    public int? ManagerId { get; set; }
    [ValidateNever]
    public int? HODId { get; set; }

    [ValidateNever]
    public string FormName { get; set; }
    [ValidateNever]
    public ClientKeyValues ClientKeyValue { get; set; } = null!;

}

