using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SimpliHR.Infrastructure.Models.Leave;

public partial class LeaveCompOffDTO
{

    public int CompOffId { get; set; }

    public string? LeavePolicy { get; set; }

    public string? EncryptedId { get; set; }
    public int? CalendarYear { get; set; }

    public string? CalendarName { get; set; }
    public TimeSpan? MinHalfDay { get; set; }

    public TimeSpan? MinFullDay { get; set; }

    public int? ApplicableDay { get; set; }

    public int? AvailDay { get; set; }

    public string? ApplicableFor { get; set; }
    public int? UnitId { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }   
    public DateTime? CreatedOn { get; set; }  
    public string? ModifiedBy { get; set; }   
    public DateTime? ModifiedOn { get; set; }
    public int? EmployeeId { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
    public List<LeaveCompOffDTO>? LeaveCompOffList { get; set; }
    public LeaveAttributeMasterKeyValues? LeaveAttributeKeyValues { get; set; }
    public int? HttpStatusCode { get; set; } = 200;

}

