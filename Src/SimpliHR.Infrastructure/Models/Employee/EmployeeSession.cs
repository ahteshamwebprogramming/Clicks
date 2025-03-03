using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee
{
    public class EmployeeSession
    {
        public int EmployeeId { get; set; }
        public string? EnycEmployeeId { get; set; }

        public int? ClientId { get; set; }
        public int? UnitId { get; set; }

        public string? EmployeeCode { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string EmployeeName { get; set; } = null!;

        public DateTime? Dob { get; set; }

        

        
    }
}
