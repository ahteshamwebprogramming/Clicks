using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Attendance;


public partial class GpslocationDetailDTO
{
    public int GpslocationId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? DutyDate { get; set; }

    public string? HostIp { get; set; }

    public string? HostName { get; set; }

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public string? AttendanceType { get; set; }
    public string DisplayMessage { get; set; } = "_Blank";

    public string? CheckInLocation { get; set; } = "";
    public string? CheckOutLocation { get; set; } = "";
}

