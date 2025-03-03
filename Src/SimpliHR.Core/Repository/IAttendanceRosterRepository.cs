using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Core.Repository;

public interface IAttendanceRosterRepository : IDapperRepository<AttendanceRoster>
{
    Task<string> SaveAttendanceShiftRoster(AttendanceRoster attendanceRosterDTO, List<EmployeeKeyValues> rosterEmployees, int unitId);
    Task<string> DeleteRosterDetails(int rosterId);

}

