using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class LoginDetail
{
    public int LoginId { get; set; }

    public int? EmployeeId { get; set; }

    public string? UserName { get; set; }

    public string? MobileNo { get; set; }
    public int? ClientId { get; set; }

    public string? Password { get; set; }


    public bool? JoiningMailSent { get; set; }

    public bool? IsPasswordSetByUser { get; set; }
    public int? LoginType { get; set; }

    public bool? IsActive { get; set; }

    public virtual Client? Client { get; set; }
}
