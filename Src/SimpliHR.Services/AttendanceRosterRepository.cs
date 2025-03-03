using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime;

namespace SimpliHR.Services;

public class AttendanceRosterRepository : DapperGenericRepository<AttendanceRoster>, IAttendanceRosterRepository
{
    //private readonly IUnitOfWork _unitOfWork;
    public AttendanceRosterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
       // _unitOfWork = unitOfWork;
    }

    public async Task<string> SaveAttendanceShiftRoster(AttendanceRoster attendanceRosterDTO, List<EmployeeKeyValues> rosterEmployees,int unitId)
    {
     
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success",sQuery=string.Empty;
        bool isEdit = false;
        if (!attendanceRosterDTO.RosterId.Equals(0))
        {
            isEdit = true;
        }
        string employeeIDs = string.Empty;
        if (rosterEmployees.Count > 0)
        {
            employeeIDs = string.Join(",", rosterEmployees.Select(x => x.EmployeeId.ToString()).ToArray());
            //employeeIDs = "3";
            if (employeeIDs.Equals(string.Empty))
                return "No Employees selected";
        }


        if (attendanceRosterDTO.StartDate == null)
            return "Select Start date for the shift roster";
        
        if (attendanceRosterDTO.EndDate == null)
            attendanceRosterDTO.EndDate = attendanceRosterDTO.StartDate;

       if ((attendanceRosterDTO.StartDate.Value - attendanceRosterDTO.EndDate.Value).TotalDays>=0)
            return "Start date cannot be greater then End Date";

        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {

                //IDBConn.Open();
                List<AttendanceHistory> attendanceHistoryList = new List<AttendanceHistory>();
                List<AttendanceHistory> attendanceHistoryUpdateList = new List<AttendanceHistory>();
                ShiftMaster shiftMaster = (await GetTableData<ShiftMaster>(IDBConn, trans, $"ShiftCode = '{attendanceRosterDTO.ShiftCode}' AND UnitId={unitId}", "")).FirstOrDefault();
                if (shiftMaster == null)
                {
                    //trans.Rollback();
                    return "Shift not found";
                }
                int id = 0;
                if (isEdit)
                {
                    sWhere = $"EntrySourceId=@entrySourceId AND EntrySource=@entrySource";
                    object paramObj = new { @entrySourceId = attendanceRosterDTO.RosterId, @entrySource = "R"};
                    bool isDeleted = await DeleteTableData<AttendanceHistory>(IDBConn, trans, sWhere, paramObj);
                    if (!isDeleted)
                    {
                        //trans.Rollback();
                        return "Failed to update roster. Error occured while deleting exiting attendance";
                    }

                    if (!(await ExecuteUpdateAsync(attendanceRosterDTO, IDBConn, trans)))
                    {
                        //trans.Rollback();
                        return "Failed to update. Roster detail not found.";
                    }
                }
                else
                {
                    id = await ExecuteAddAsync(attendanceRosterDTO, IDBConn, trans);
                    if (id == 0)
                    {
                        //trans.Rollback();
                        return "Fails to save roster.";
                    }
                        
                    attendanceRosterDTO.RosterId = id;
                }
                List<AttendanceHistory> listOfAttendance = new List<AttendanceHistory>();
                string startDate = attendanceRosterDTO.StartDate.Value.ToString("MM/dd/yyyy");
                string endDate = attendanceRosterDTO.EndDate.Value.ToString("MM/dd/yyyy");
            if (employeeIDs.Equals(string.Empty))
                    sWhere = $"((DutyDate BETWEEN '{startDate}' AND '{endDate}')  AND UnitId in ({unitId}))";
                else
                    sWhere = $"((DutyDate BETWEEN '{startDate}' AND '{endDate}')  AND EmployeeId in ({employeeIDs}))";

                //Get the weeklyoff days
                UnitMaster unitInfo = (await GetTableData<UnitMaster>(IDBConn, trans, $" UnitId={unitId}", "")).FirstOrDefault();
                string weeklyOffStr = unitInfo.WeeklyOff;
                listOfAttendance.AddRange(await GetTableData<AttendanceHistory>(IDBConn, trans, sWhere, sOrderBy));
                //if (listOfAttendance.Count > 0)
                //    sReturnMsg = string.Empty;
                var selectedDates = CommonHelper.EachDateBetweenDates((DateTime)attendanceRosterDTO.StartDate, (DateTime)attendanceRosterDTO.EndDate);
                sQuery = $"SELECT UnitHolidayId, HolidayID, UnitId, HolidayDate, HolidayMonth, HolidayDay, HolidayYear, HolidayName, HolidayType, IsActive, CreatedBy, CreatedOn, ModifedBy, ModifiedOn FROM UnitHolidayList " +
                    $" WHERE (HolidayDate BETWEEN @startDate AND @endDate) AND UnitId in (@unitId) AND IsActive=1";

                List<HolidaysListMaster> holidayDates = (await GetTableData<HolidaysListMaster>(sQuery, new { @startDate = startDate, @endDate = endDate, @unitId = unitId }, IDBConn, trans));
                
                foreach (DateTime shiftDate in selectedDates)
                {
                   
                    string sShiftDate = shiftDate.ToString("dd-MMM-yyyy");

                    foreach (var item in rosterEmployees)
                    {
                        //Code is added for Shift should be created between DOJ and ExitDate
                        if ((item.DOJ != null && shiftDate >= item.DOJ) && (item.ExitDate == null || (item.ExitDate != null && shiftDate <= item.ExitDate)))
                        {
                            if (listOfAttendance.Find(x => x.EmployeeId == item.EmployeeId && x.DutyDate == shiftDate) == null)
                            {
                                attendanceHistoryList.Add(await GetAttendanceData(item, attendanceRosterDTO, shiftMaster, shiftDate, holidayDates, weeklyOffStr));
                            }
                            else
                            {
                                //sReturnMsg += item.EmployeeId.ToString() + " [" + item.EmployeeName + "]" + " Duty Date "+ sShiftDate + " Your selected shift " + shiftMaster.ShiftCode + " Already ScheduledShift " + listOfAttendance.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.ShiftIDScheduled).FirstOrDefault() + CommonHelper.NewLineEntry();
                                attendanceHistoryUpdateList.Add(await GetAttendanceData(item, attendanceRosterDTO, shiftMaster, shiftDate, holidayDates, weeklyOffStr));
                            }
                        }
                            
                       // }
                        
                        var isShiftOverlapping = await CheckShiftOverlapping(IDBConn,trans, listOfAttendance, attendanceHistoryList, shiftDate, item);
                        if (isShiftOverlapping)
                        {
                            DateTime prevDate = shiftDate.AddDays(-1);
                            return $"Shift overlapping not allowed. Overlapping shift found for employee {item.EmployeeName} duty date {prevDate.ToString("dd-MMM-yy")} and{shiftDate.ToString("dd-MMM-yy")}. Roster saving cancelled";
                        }                           
                    }
                }
                if (attendanceHistoryList.Count > 0)
                {
                    IDBConn.Execute(@"
                        insert AttendanceHistory(EmployeeID,DutyDate,WorkLocationId,UnitId,ShiftMonth,ShiftYear,ShiftIDScheduled,ShiftIDAttended,ShiftStartTime, ShiftEndTime,EntrySource,EntrySourceId,AttendanceType,IsHoliday)
                        values(@EmployeeID, @DutyDate,@WorkLocationId, @UnitId,@ShiftMonth,@ShiftYear,@ShiftIdscheduled,@ShiftIDAttended,@ShiftStartTime, @ShiftEndTime,@EntrySource,@EntrySourceId,@AttendanceType,@IsHoliday)", attendanceHistoryList, trans);
                }
                if (attendanceHistoryUpdateList.Count > 0)
                {
                    IDBConn.Execute(@"
                        Update AttendanceHistory SET WorkLocationId=@WorkLocationId,ShiftIDScheduled=@ShiftIdscheduled,@ShiftIDAttended=@ShiftIdscheduled,ShiftStartTime=@ShiftStartTime,ShiftEndTime=@ShiftEndTime,EntrySource=@EntrySource,EntrySourceId=@EntrySourceId,AttendanceType=@AttendanceType,IsHoliday=@IsHoliday
                        WHERE EmployeeId=@EmployeeId AND DutyDate=@DutyDate", attendanceHistoryUpdateList, trans);
                }

                    //if (attendanceHistoryList.Count == 0 && (!isEdit))
                    //    trans.Rollback();
                    //else
                    //    trans.Commit();
            }
            return sReturnMsg;
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return "fail to save roster. Error occured while saving roster.";
        }
    }
    public async Task<AttendanceHistory> GetAttendanceData(EmployeeKeyValues item, AttendanceRoster attendanceRosterDTO, ShiftMaster shiftMaster,DateTime shiftDate,List<HolidaysListMaster> holidayDates,string weeklyOffStr)
    {
        return new AttendanceHistory
        {
            EmployeeId = item.EmployeeId,
            UnitId = attendanceRosterDTO.UnitId,
            ShiftIDScheduled = shiftMaster.ShiftCode,
            ShiftIDAttended = shiftMaster.ShiftCode,
            WorkLocationId = attendanceRosterDTO.WorkLocationId,
            ShiftStartTime = CommonHelper.StringToDateTime(shiftDate.Date.ToShortDateString() + " " + shiftMaster.InTime),
            ShiftEndTime = ((bool)shiftMaster.IsNightShift ? CommonHelper.StringToDateTime((shiftDate.AddDays(1).Date.ToShortDateString() + " " + shiftMaster.OutTime)) : CommonHelper.StringToDateTime((shiftDate.Date.ToShortDateString() + " " + shiftMaster.OutTime))),
            DutyDate = (DateTime)CommonHelper.StringToDateTime(shiftDate.ToShortDateString()),
            ShiftMonth = shiftDate.Date.Month,
            ShiftYear = shiftDate.Date.Year,
            AttendanceType = ("," + weeklyOffStr + ",").Contains(((int)shiftDate.DayOfWeek).ToString()) ? "WO" : null,
            IsHoliday = ((holidayDates != null && holidayDates.Count > 0 && holidayDates.Where(x => x.HolidayDate.Value == shiftDate).Count() > 0) ? true : false),
            EntrySource = "R",
            EntrySourceId = attendanceRosterDTO.RosterId
        };
    }

    [HttpGet]
    public async Task<string> DeleteRosterDetails(int rosterId)
    {
        bool isDeleted = false;
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success", sQuery = string.Empty;

        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                sWhere = "EntrySourceId=@rosterId AND EntrySource = @entrySource";
                //IDBConn.Execute("DELETE FROM AttendanceHistory WHERE EntrySourceId=@rosterId", new { @rosterId = rosterId }, trans);

                isDeleted = await DeleteTableData<AttendanceHistory>(IDBConn, trans, sWhere, new { @rosterId = rosterId, @entrySource = "R" });
                //RunSQLCommand("DELETE FROM AttendanceHistory WHERE EntrySourceId="+ rosterId +" AND EntrySource='R'");
                if (isDeleted)
                    if (await DeleteTableData<AttendanceRoster>(IDBConn, trans, "RosterId=@rosterId", new { @rosterId = rosterId }))
                    {
                        trans.Commit();
                        sReturnMsg = "Success";
                    }
            }
        }
        catch (Exception ex)
        {
            //trans.Rollback();
            sReturnMsg= "fail to delete roster. Error occured while deleting roster.";
        }


        return sReturnMsg;

    }

    public async Task<bool> CheckShiftOverlapping(IDbConnection IDBConn, IDbTransaction trans, List<AttendanceHistory> attendanceHistoryDBList, List<AttendanceHistory> attendanceHistoryList, DateTime shiftDate, EmployeeKeyValues item)
    {
        bool isOverLappingShift=false;
        DateTime prevDate = shiftDate.AddDays(-1);
        int employeeId = item.EmployeeId;
        AttendanceHistory curShiftData = attendanceHistoryList.Where(r => r.DutyDate == shiftDate && r.EmployeeId == item.EmployeeId).Select(p => p).FirstOrDefault();
        AttendanceHistory prevShiftData = attendanceHistoryList.Where(r => r.DutyDate == prevDate && r.EmployeeId==item.EmployeeId).Select(p => p).FirstOrDefault();
        //curShiftData.ShiftStartTime = DateTime.ParseExact("2024-06-01 09:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);
        //curShiftData.ShiftEndTime = DateTime.ParseExact("2024-06-01 06:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);
        if(curShiftData==null)
        {
            curShiftData = attendanceHistoryDBList.Where(r => r.DutyDate == shiftDate && r.EmployeeId == item.EmployeeId).Select(p => p).FirstOrDefault();
        }          
        if (prevShiftData == null)
        {
            prevShiftData = attendanceHistoryList.Where(r => r.DutyDate == prevDate && r.EmployeeId == item.EmployeeId).Select(p => p).FirstOrDefault();
        }
        if (prevShiftData == null)
        {
            prevShiftData = (await GetTableData<AttendanceHistory>("SELECT ShiftIDScheduled,InTime,OutTime FROM AttendanceHistory WHERE EmployeeId=@employeeId AND DutyDate=@dutyDate ", new { @employeeId = employeeId, @dutyDate = prevDate }, IDBConn, trans)).FirstOrDefault();
        }
        //prevShiftData.ShiftStartTime = DateTime.ParseExact("2024-06-02 09:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); ;
        //prevShiftData.ShiftEndTime = DateTime.ParseExact("2024-06-02 06:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);
        //prevShiftData.DutyDate = DateTime.ParseExact("2025-06-01 06:00:00", "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);

        if (curShiftData!=null && prevShiftData != null && prevShiftData.DutyDate.ToString()!= "01-01-0001 00:00:00")
            
            if (curShiftData.ShiftStartTime >= prevShiftData.ShiftStartTime &&  curShiftData.ShiftStartTime <= prevShiftData.ShiftEndTime 
                || curShiftData.ShiftEndTime >= prevShiftData.ShiftStartTime && curShiftData.ShiftEndTime<=prevShiftData.ShiftEndTime)
                isOverLappingShift=true;
        else if (prevShiftData.ShiftStartTime >= curShiftData.ShiftStartTime && prevShiftData.ShiftStartTime <= curShiftData.ShiftEndTime
                || prevShiftData.ShiftEndTime >= curShiftData.ShiftStartTime && prevShiftData.ShiftEndTime <= curShiftData.ShiftEndTime)
            isOverLappingShift = true;
        return isOverLappingShift;
    }

}


