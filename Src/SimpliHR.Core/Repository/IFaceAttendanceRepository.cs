using Dapper;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Attendance;

namespace SimpliHR.Core.Repository;

public interface IManualPunchesRepository : IDapperRepository<ManualPunches>
{
    Task<string> ProcessManualPunches(List<AttendanceHistory> attendanceHistoryData);
    Task<string> SendManualPunchesforApproval(List<AttendanceHistory> attendanceHistoryData,string ProfilePic, string DisplayName, int EmailProvider);

    Task<string> SendManualPunchActionMail(ManualPunchesAction userAction);

    Task<List<AttendanceHistory>> GetAttendance(string spName, DynamicParameters param);
}

