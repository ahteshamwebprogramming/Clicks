using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeContactDetailDTO
{
    public int EmployeeContactDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Landmark { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public int? CityId { get; set; }

    public string? Pincode { get; set; }

    public string? ContactNum { get; set; }

    public string? ContactType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }

    public string FormName { get; set; }
    public int? ClientId { get; set; }

    public CountryMasterDTO Country { get; set; }=new CountryMasterDTO();

    public StateMasterDTO State { get; set; } = new StateMasterDTO();

    public CityMasterDTO City { get; set; } = new CityMasterDTO();

    public string? DisplayMessage { get; set; } = "_blank";
}
