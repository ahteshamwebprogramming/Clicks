using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public class EmployeeLanguageDetailDTO
{
    public int EmployeeLanguageDetailId { get; set; }

    public int EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? LanguageId { get; set; }

    public string? Language { get; set; }

    public string? TicketId { get; set; }
    public string? EntrySource { get; set; }

    public bool? Read { get; set; }

    public bool? Write { get; set; }

    public bool? Speak { get; set; }

    public string? CanRead { get; set; }

    public string? CanWrite { get; set; }

    public string? CanSpeak { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public LanguageMasterDTO LanguageMaster { get; set; } = new LanguageMasterDTO();
    public EmployeeMasterDTO? Employee { get; set; }
}
