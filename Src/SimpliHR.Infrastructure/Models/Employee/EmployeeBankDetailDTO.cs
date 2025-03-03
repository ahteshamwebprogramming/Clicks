using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeBankDetailDTO
{
    public int BankDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }

    public int? BankId { get; set; }

    public string? AccountName { get; set; }

    public string? AccountNo { get; set; }

    public string? IFSCCode { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    //public BankMasterDTO Bank { get; set; } = null!;

    public EmployeeMasterDTO? Employee { get; set; }

    public string FormName { get; set; }
    public int? ClientId { get; set; }

    public BankMasterDTO Bank { get; set; } = new BankMasterDTO();
}
