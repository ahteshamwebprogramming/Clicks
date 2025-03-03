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

public partial class LeaveCalenderYearDTO
{

    public int LeaveYearId { get; set; }
    public string EncryptedId { get; set; }
    public string? CalendarName { get; set; }

     public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public int? UnitId { get; set; }
    public bool? IsActive { get; set; }

     public string? CreatedBy { get; set; }

   
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }

   
    public DateTime? ModifiedOn { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<LeaveCalenderYearDTO>? LeaveCalenderYearList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;

}

