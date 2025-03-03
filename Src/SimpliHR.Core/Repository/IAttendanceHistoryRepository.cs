
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Attendance;

namespace SimpliHR.Core.Repository;

public interface IAttendanceHistoryRepository : IDapperRepository<AttendanceHistory>
{
    Task<string> SaveEmployeeShiftDetails(List<AttendanceHistory> attendanceHistoryData,int unitId);
    Task<string> LockAttendnace(LockAttendanceDTO lockAttendance, DateTime? startDate, DateTime? endDate);
}

