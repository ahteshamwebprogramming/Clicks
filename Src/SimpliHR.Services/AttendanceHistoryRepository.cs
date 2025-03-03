using SimpliHR.Services.DBContext;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Helper;
using System.Data;
using Dapper;
using SimpliHR.Infrastructure.Models.Attendance;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace SimpliHR.Services;

public class AttendanceHistoryRepository : DapperGenericRepository<AttendanceHistory>, IAttendanceHistoryRepository
{
    public AttendanceHistoryRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }

    public async Task<string> SaveEmployeeShiftDetails(List<AttendanceHistory> attendanceHistoryData, int unitId)
    {
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success", status;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                List<ShiftMaster> shiftMaster = new List<ShiftMaster>();
                string shiftCodes = string.Join(",", attendanceHistoryData.DistinctBy(r => r.ShiftIDScheduled).Select(x => $"'{x.ShiftIDScheduled}'"));
                if (shiftCodes.Length > 0)
                {
                    string sQuery = $"SELECT a.ShiftId,a.ShiftCode,a.ShiftName, a.UnitId, a.InTime,a.OutTime,a.IsNightShift FROM ShiftMaster a " +
                                    $" WHERE a.ShiftCode in({shiftCodes}) AND a.UnitId={unitId} AND a.IsActive=1";
                    shiftMaster = (await GetTableData<ShiftMaster>(sQuery, IDBConn, trans));
                }
                if (shiftMaster.Count == 0 && shiftMaster == null)
                    return "Shift not found. Fail to save shifts.";
                foreach (var attendance in attendanceHistoryData)
                {
                    DateTime? shiftStartAt, shiftEndtAt;
                    //int id = await EexecuteAddAsync(punch, IDBConn, trans);
                    sWhere = $"EmployeeId={attendance.EmployeeId} AND DutyDate = '{attendance.DutyDate.ToString("yyyy-MM-dd")}'";
                    AttendanceHistory attendanceDBData = (await GetTableData<AttendanceHistory>(IDBConn, trans, sWhere, "")).FirstOrDefault();
                    var shifInfo = shiftMaster.Where(r => r.ShiftCode == attendance.ShiftIDScheduled);
                    shiftStartAt = CommonHelper.StringToDateTime(attendance.DutyDate.ToString("yyyy-MM-dd") + " " + shifInfo.Select(x => x.InTime).FirstOrDefault().Value);
                    shiftEndtAt = CommonHelper.StringToDateTime(((shifInfo.Select(x => x.IsNightShift).FirstOrDefault().Value == true) ? attendance.DutyDate.AddDays(1).ToString("yyyy-MM-dd") : attendance.DutyDate.ToString("yyyy-MM-dd")) + " " + shifInfo.Select(x => x.OutTime).FirstOrDefault().Value);

                    if (attendanceDBData != null)
                        attendance.EntrySource = "SU";
                    else
                        attendance.EntrySource = "SS";

                    //if ((attendance.InTime.Value == null || attendance.InTime.Value.ToString() == "00:00:00"))
                    //    attendance.ShiftIDAttended = attendance.ShiftIDScheduled;

                    attendance.ShiftStartTime = shiftStartAt;
                    attendance.ShiftEndTime = shiftEndtAt;
                    attendance.ShiftMonth = attendance.DutyDate.Month;
                    attendance.ShiftYear = attendance.DutyDate.Year;
                    attendance.UnitId = unitId;
                    //attendance.ShiftIDAttended
                }
                List<AttendanceHistory> attendaceToCreate = new List<AttendanceHistory>();
                attendaceToCreate = attendanceHistoryData.Where(x => x.EntrySource != null && x.EntrySource == "SS").ToList();
                if (attendaceToCreate.Count > 0)
                {
                    await IDBConn.ExecuteAsync(@"INSERT INTO AttendanceHistory(DutyDate,EmployeeId,UnitId,WorkLocationId,ShiftIdScheduled,ShiftIDAttended,ShiftMonth,ShiftYear,ShiftStartTime,ShiftEndTime,EntrySource) 
                    VALUES(@DutyDate,@EmployeeId,@UnitId,(SELECT WorkLocationId from EmployeeMaster Where EmployeeId=@EmployeeId),@ShiftIDScheduled,@ShiftIDScheduled,@ShiftMonth,@ShiftYear,@ShiftStartTime,@ShiftEndTime,@EntrySource)", attendaceToCreate, trans);

                }
                List<AttendanceHistory> attendaceToUpdate = new List<AttendanceHistory>();
                attendaceToUpdate = attendanceHistoryData.Where(x => x.EntrySource != null && x.EntrySource == "SU").ToList();

                if (attendaceToUpdate.Count > 0)
                {
                    await IDBConn.ExecuteAsync(@"UPDATE AttendanceHistory SET ShiftIDScheduled=@ShiftIDScheduled,ShiftIDAttended=@ShiftIDScheduled,ShiftMonth=@ShiftMonth,ShiftYear=@ShiftYear,ShiftStartTime=@ShiftStartTime,ShiftEndTime=@ShiftEndTime,@EntrySource=EntrySource
                    WHERE DutyDate=@DutyDate AND EmployeeId=@EmployeeId ", attendaceToUpdate, trans);
                    //trans.Commit();
                }
                //if(attendaceToUpdate.Count>0 || attendaceToCreate.Count > 0)
                //    trans.Commit();

                return "Success";
            }
        }
        catch (Exception ex)
        {
            //trans.Rollback();
            return "fail to save roster. Error occured while saving roster.";
        }
    }

    public async Task<string> LockAttendnace(LockAttendanceDTO lockAttendance, DateTime? startDate, DateTime? endDate)
    {
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success", status,sQuery=string.Empty;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                sQuery = $"SELECT * FROM LockAttendance WHERE LockMonth=@LockMonth AND LockYear=@LockYear AND UnitId=@unitId";
                LockAttendanceDTO lockAttendanceData = (await GetTableData<LockAttendanceDTO>(sQuery,new { @LockMonth = lockAttendance.LockMonth,@LockYear = lockAttendance.LockYear, @unitId = lockAttendance.UnitId }, IDBConn, trans)).FirstOrDefault();
                if (lockAttendanceData != null)
                {
                    lockAttendance.ModifiedBy = lockAttendance.CreatedBy;
                    lockAttendance.ModifiedOn = lockAttendance.CreatedOn;
                    sQuery = "UPDATE LockAttendance SET AttendanceByPass=@AttendanceByPass,RunLeaveScheduler=@RunLeaveScheduler,ModifiedBy=@ModifiedBy,ModifiedOn=@ModifiedOn WHERE LockMonth=@LockMonth AND LockYear=@LockYear AND UnitId=@UnitId ";
                    await IDBConn.ExecuteAsync(sQuery, lockAttendance, trans);
                }
                else
                {
                    sQuery = "INSERT INTO LockAttendance(UnitId,LockMonth,LockYear,AttendanceByPass,RunLeaveScheduler,CreatedBy,CreatedOn) " +
                       " VALUES (@UnitId,@LockMonth,@LockYear,@AttendanceByPass,@RunLeaveScheduler,@CreatedBy,@CreatedOn)";
                    await IDBConn.ExecuteAsync(sQuery, lockAttendance, trans);
                    
                }
                sQuery = $"UPDATE AttendanceHistory SET IsAttendanceLocked=1 WHERE DutyDate BETWEEN @startDate AND @endDate AND UnitId=@unitId";
                await IDBConn.ExecuteAsync(sQuery, new { @startDate=startDate.Value.ToString("MM/dd/yyyy"),@endDate= endDate.Value.ToString("MM/dd/yyyy"), @unitId = lockAttendance.UnitId },trans);
                trans.Commit();
            }
            return "Success";
        }
        catch (Exception ex)
        {
            return "Eror occured while locking attendance";
        }
    }
            
}


