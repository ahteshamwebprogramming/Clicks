using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public class EditEmployeeDataViewModel
{
    public List<EditEmployeeDataDTO> EditEmployeeDataList { get; set; }
    //public List<IFormFile> Files { get; set; }
}
