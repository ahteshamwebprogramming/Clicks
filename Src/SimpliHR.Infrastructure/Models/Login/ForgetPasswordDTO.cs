using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Login;

public class ForgetPasswordDTO
{
    [ValidateNever]
    public string NewPassword { get; set; }
    [ValidateNever]
    [Required(ErrorMessage = "Please enter password to confirm"), MinLength(6, ErrorMessage = "Password Length should be min 6 characters")]
    [Compare("NewPassword", ErrorMessage = "Password and Confirmation Password must match.")]
    public string ConfirmPassword { get; set; }
    [ValidateNever]
    public string NewPasswordEncrypted { get; set; }

    [Required(ErrorMessage = "Please enter Email ")]
    public string EmployeeEmail { get; set;}

    [ValidateNever]
    public int? EmployeeId { get; set; }

    [ValidateNever]
    public string DisplayMessage { get; set; }
    [ValidateNever]
    public string EncryptedEmpId { get; set; }
}
