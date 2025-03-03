using Dapper;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Attendance;

namespace SimpliHR.Core.Repository;

public interface IFaceAttendanceRepository : IDapperRepository<FaceAttendance>
{
    
}

