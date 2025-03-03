using Dapper;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Attendance;

namespace SimpliHR.Core.Repository;

public interface IEmployeesSalaryProcessDetailsRepository : IDapperRepository<EmployeesSalaryProcessDetails>
{
    //Task<string> ProcessManualPunches(List<AttendanceHistory> attendanceHistoryData);
    //Task<string> SendManualPunchesforApproval(List<AttendanceHistory> attendanceHistoryData);

    //Task<string> SendManualPunchActionMail(ManualPunchesAction userAction);

    //Task<List<AttendanceHistory>> GetAttendance(string spName, DynamicParameters param);
}

