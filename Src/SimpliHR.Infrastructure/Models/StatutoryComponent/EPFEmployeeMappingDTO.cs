﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.StatutoryComponent;

public partial class EPFEmployeeMappingDTO
{
    public int StatutoryComponentsId { get; set; }
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; }

}