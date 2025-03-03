using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee
{
    public class EmployeeTicketViewModelDTO
    {
        public int? TicketId { get; set; }
        public string? encTicketId { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }

    }
}
