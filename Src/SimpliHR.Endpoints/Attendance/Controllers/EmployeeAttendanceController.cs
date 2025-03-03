using AutoMapper;
using AutoMapper.Execution;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Printing;
using System.Dynamic;
using System.Linq.Expressions;

namespace SimpliHR.Endpoints;

[ApiController]
[Route("[controller]")]
public class EmployeeAttendanceController : ControllerBase
{

    private readonly DapperDBContext _dapperDBContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeAttendanceController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;


    public EmployeeAttendanceController(IUnitOfWork unitOfWork, ILogger<EmployeeAttendanceController> logger, IMapper mapper,DapperDBContext dapperDBContext, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBContext = dapperDBContext;
        _configuration = configuration;
    }

    public async Task<AttendanceHistoryViewModel> GetEmployeeAttendance(AttendanceHistoryViewModel attendanceVM, int limit, int offset)
    {
        try
        {
            List<AttendanceHistoryDTO> outputModel = new List<AttendanceHistoryDTO>();
            if ((attendanceVM.StartDate.Value - attendanceVM.EndDate.Value).TotalDays > 0)
                attendanceVM.DisplayMessage = "Start date cannot be greater then End Date";
            else
            {
                //string sWhere = $"((DutyDate BETWEEN '{CommonHelper.StringToDateOnly(attendanceVM.StartDate.Value.ToString("MM/dd/yyyy"))}' AND '{CommonHelper.StringToDateOnly(attendanceVM.EndDate.Value.ToString("MM/dd/yyyy"))}') AND  EmployeeId = {attendanceVM.EmployeeId})";
                string sWhere = $" WHERE ((a.DutyDate BETWEEN '{attendanceVM.StartDate.Value.ToString("yyyy-MM-dd")}' AND '{attendanceVM.EndDate.Value.ToString("yyyy-MM-dd")}') AND  a.EmployeeId = {attendanceVM.EmployeeId})";
                string sOrderBy = " ORDER BY a.DutyDate";
                string sQuery = "Select a.*,b.ActionType,b.ActionBy,b.ActionOn,b.ActionRemark,b.ManualPunchReason,b.TicketId,(SELECT c.EmployeeName FROM EmployeeMaster c WHERE c.EmployeeId=b.ActionBy) ActionByName " +
                    " FROM AttendanceHistory a LEFT JOIN ManualPunches b ON a.DutyDate = b.ManualPunchDate AND a.EmployeeId=b.EmployeeId AND (b.ActionType IN('A','R')  OR b.ActionType IS NULL)" + sWhere + sOrderBy;
                attendanceVM.AttendanceList = _mapper.Map<List<AttendanceDTO>>(await _unitOfWork.AttendanceHistory.GetTableData<AttendanceDTO>(sQuery));
                attendanceVM.DutyDays = attendanceVM.AttendanceList.Count();
                attendanceVM.HalfDays = ((float)attendanceVM.AttendanceList.Where(x => (x.Status!=null && x.Status.Trim().ToUpper() == "P/H")).Count()) / 2;
                attendanceVM.PresentDays = attendanceVM.AttendanceList.Where(x => (x.Status != null && x.Status.Trim().ToUpper() == "P")).Count();
                attendanceVM.PresentDays += attendanceVM.HalfDays;
                attendanceVM.AbsentDays = attendanceVM.AttendanceList.Where(x => (x.Status == null || x.Status.Trim().ToUpper() == "A")).Count();
                attendanceVM.AbsentDays += attendanceVM.HalfDays;
                attendanceVM.OutsideDuty = attendanceVM.AttendanceList.Where(x => (x.AttendanceType!=null && x.AttendanceType.Trim().ToUpper() == "OD")).Count();
                attendanceVM.WeeklyOff = attendanceVM.AttendanceList.Where(x => ((!x.IsHoliday.Value) && x.AttendanceType != null && x.AttendanceType.Trim().ToUpper() == "WO")).Count();
                attendanceVM.Holidays = attendanceVM.AttendanceList.Where(x => (x.IsHoliday.Value)).Count();
                attendanceVM.Leaves = 0;
                attendanceVM.Approved = 0;
                attendanceVM.Rejected = 0;
                sQuery = $"SELECT DISTINCT a.EmployeeId,a.DutyDate, (SELECT GPSLocation From GPSLocationDetails WHERE GpslocationId=(SELECT DISTINCT MIN(GpslocationId) " +
                    $" FROM GPSLocationDetails a1 WHERE a1.EmployeeId=a.EmployeeId and a1.DutyDate=a.DutyDate AND a1.AttendanceType='ClockIn')) CheckInLocation," +
                    $" (SELECT GPSLocation From GPSLocationDetails WHERE GpslocationId=(SELECT DISTINCT MAX(GpslocationId) FROM GPSLocationDetails a2 " +
                    $" WHERE a2.EmployeeId=a.EmployeeId and a2.DutyDate=a.DutyDate AND a2.AttendanceType='ClockOut')) CheckOutLocation FROM GPSLocationDetails a" + sWhere ;
                attendanceVM.GpslocationDetailList = await _unitOfWork.AttendanceHistory.GetTableData<GpslocationDetailDTO>(sQuery);
                attendanceVM.AttendanceList.Skip(offset * limit).Take(limit).ToList();
            }
            return attendanceVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeAttendance)}");
            throw;
        }
    }

    public async Task<LockAttendanceDTO> LockAttendance(LockAttendanceDTO lockAttendance)
    {
       
       UnitMaster unitMaster = await GetUnitPayrollDates(lockAttendance.UnitId);
        var tempLockMonth = lockAttendance.LockMonth;
        if (lockAttendance.LockMonth == 1)
        {
            lockAttendance.LockMonth = 12;
            lockAttendance.LockYear = lockAttendance.LockYear - 1;
        }
        else
            lockAttendance.LockMonth = lockAttendance.LockMonth - 1;

       DateTime? startDate = new DateTime(lockAttendance.LockYear, lockAttendance.LockMonth, 1);
       DateTime? endDate = startDate.Value.AddMonths(1).AddDays(-1);
        int lastDayOfMonth = DateTime.DaysInMonth(lockAttendance.LockYear, lockAttendance.LockMonth);

        if (unitMaster != null)
        {
            startDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? startDate : new DateTime(lockAttendance.LockYear, lockAttendance.LockMonth, unitMaster.PayrollStartDate.Value);
            int dayDiff = unitMaster.PayrollEndDate.Value - unitMaster.PayrollStartDate.Value;
            //int iMonth = (DateTime.Today.AddMonths(1)).Month;
            if (dayDiff <= 0)
                endDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? endDate : new DateTime(lockAttendance.LockYear, (startDate.Value.AddMonths(1)).Month, unitMaster.PayrollEndDate.Value);
            else if (unitMaster.PayrollEndDate.Value > lastDayOfMonth)
                endDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? endDate : new DateTime(lockAttendance.LockYear, (lockAttendance.LockMonth), lastDayOfMonth);
            else
                endDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? endDate : new DateTime(lockAttendance.LockYear, lockAttendance.LockMonth, unitMaster.PayrollEndDate.Value);
        }
        lockAttendance.LockMonth = tempLockMonth;
        lockAttendance.DisplayMessage = await _unitOfWork.AttendanceHistory.LockAttendnace(lockAttendance,startDate,endDate);
        return lockAttendance;
    }

    public async Task<UnitMaster> GetUnitPayrollDates(int? unitId)
    {
        UnitMaster unitInfo = new UnitMaster();
        Expression<Func<UnitMaster, bool>> expression = a => a.UnitID == unitId && a.IsActive == true;
        unitInfo = _unitOfWork.UnitMaster.FindFirstByExpression(expression);
        return unitInfo;
    }

    public async Task<SortedDictionary<string, string>> GetWeekDays(string unitIds)
    {
        int iKey = 0, unitId = 0;
        bool success = int.TryParse(unitIds, out unitId);
        //  Dictionary<int, string> collectionWeekDays = new Dictionary<int, string>();
        SortedDictionary<string, string> collectionWeekDays = new SortedDictionary<string, string>();
        UnitMasterDTO unitMaster = new UnitMasterDTO();
        Expression<Func<UnitMaster, bool>> expression = a => a.UnitID == unitId && a.IsActive == true;
        unitMaster = _mapper.Map<UnitMasterDTO>(_unitOfWork.UnitMaster.FindAllByExpression(expression).FirstOrDefault());
        if (unitMaster.WeeklyOff != null && unitMaster.WeeklyOff.Length != 0)
        {
            string addText = "";
            string[] weekOffArr = unitMaster.WeeklyOff.Split(",");
            if (weekOffArr.Length >= 1)
            {
                addText = " Only";
            }
            int weekDayNo = 0, intDay;
            foreach (string weekDay in weekOffArr)
            {
                if (int.TryParse(weekDay, out weekDayNo))
                {
                    intDay = weekDayNo;
                    collectionWeekDays.Add((weekDayNo == 0 ? "7" : weekDay), ((DayOfWeek)intDay).ToString() + addText);
                }

            }
            if (collectionWeekDays.Count >= 1)
            {
                string key = string.Join(",", collectionWeekDays.Reverse().Select(x => x.Key.Replace(" ", "").Trim()));

                string value = string.Join(",", collectionWeekDays.Select(x => x.Value.Replace("Only", "").Trim())).Replace(",", " & ");
                if (!collectionWeekDays.ContainsKey(key))
                    collectionWeekDays.Add(key, value);
                collectionWeekDays.Add(((weekDayNo == 0 ? 7 : weekDayNo) + 1).ToString(), "All");
            }
        }
        return collectionWeekDays;
    }
    public async Task<AttendanceHistoryViewModel> GetShiftList(AttendanceHistoryViewModel attendanceVM, int limit = 0, int offset = 0)
    {
        try
        {
            List<AttendanceHistoryDTO> outputModel = new List<AttendanceHistoryDTO>();
            if ((attendanceVM.StartDate.Value - attendanceVM.EndDate.Value).TotalDays > 0)
                attendanceVM.DisplayMessage = "Start date cannot be greater then End Date";
            else
            {
                //string sWhere = $"((DutyDate BETWEEN '{CommonHelper.StringToDateOnly(attendanceVM.StartDate.Value.ToString("MM/dd/yyyy"))}' AND '{CommonHelper.StringToDateOnly(attendanceVM.EndDate.Value.ToString("MM/dd/yyyy"))}') AND  EmployeeId = {attendanceVM.EmployeeId})";
                string sWhere = $" WHERE  (a.DutyDate BETWEEN '{attendanceVM.StartDate.Value.ToString("yyyy-MM-dd")}' AND '{attendanceVM.EndDate.Value.ToString("yyyy-MM-dd")}')";
                sWhere = ((attendanceVM.DepartmentIds != null && attendanceVM.DepartmentIds != string.Empty) ? sWhere + $" AND b.DepartmentId IN (SELECT value FROM STRING_SPLIT('{attendanceVM.DepartmentIds}',','))" : sWhere);
                sWhere = ((attendanceVM.EmployeeIds != null && attendanceVM.EmployeeIds != string.Empty) ? sWhere + $" AND b.EmployeeId IN (SELECT value FROM STRING_SPLIT('{attendanceVM.EmployeeIds}',',') )" : sWhere);
                string sDaySelected = attendanceVM.DaySelected == "All" ? string.Empty : attendanceVM.DaySelected;
                sDaySelected = (sDaySelected != null || sDaySelected != "") ? sDaySelected.Replace(" ", "").Replace("Only", "").Replace("&", ",") : string.Empty;
                sWhere = ((sDaySelected != string.Empty) ? sWhere + $" AND DateName(WeekDay, a.DutyDate) IN (SELECT value FROM STRING_SPLIT('{sDaySelected}',',') )" : sWhere);
                string sOrderBy = " ORDER BY b.EmployeeName,a.DutyDate";
                string sQuery = "Select a.*,b.EmployeeName FROM AttendanceHistory a LEFT JOIN EmployeeMaster b ON a.EmployeeId=b.EmployeeId " + sWhere + sOrderBy;
                attendanceVM.AttendanceList = _mapper.Map<List<AttendanceDTO>>(await _unitOfWork.AttendanceHistory.GetQueryAll(sQuery));

                if (attendanceVM.AttendanceList.Count == 0)
                {
                    sWhere = ((attendanceVM.DepartmentIds != null && attendanceVM.DepartmentIds != string.Empty) ? $" WHERE b.DepartmentId IN (SELECT value FROM STRING_SPLIT('{attendanceVM.DepartmentIds}',','))" : "");
                    sWhere = sWhere + ((attendanceVM.EmployeeIds != null && attendanceVM.EmployeeIds != string.Empty) ? $" AND b.EmployeeId IN (SELECT value FROM STRING_SPLIT('{attendanceVM.EmployeeIds}',',') )" : "");
                    sOrderBy = " ORDER BY b.EmployeeName";
                    sQuery = "Select b.EmployeeName,b.EmployeeId FROM EmployeeMaster b " + sWhere + sOrderBy;
                    attendanceVM.AttendanceList = _mapper.Map<List<AttendanceDTO>>(await _unitOfWork.AttendanceHistory.GetQueryAll(sQuery));
                }

                attendanceVM.AttendanceList.Skip(offset * limit).Take(limit).ToList();
            }
            return attendanceVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeAttendance)}");
            throw;
        }
    }


    public async Task<ManualPunchesViewModel> GetAttendancePendingForApproval(int employeeId)
    {
        ManualPunchesViewModel manualPunchesVM = new ManualPunchesViewModel();
        string sQuery = $"SELECT b.*,a.ShiftIDAttended,c.EmployeeName,c.EmployeeCode FROM AttendanceHistory a INNER JOIN ManualPunches b ON a.EmployeeId=b.EmployeeId AND a.DutyDate=b.ManualPunchDate " +
            $" LEFT JOIN EmployeeMaster c ON a.EmployeeId=c.EmployeeId  WHERE b.EmployeeId IN (SELECT EmployeeId FROM EmployeeMaster WHERE ManagerId={employeeId} OR  HODId={employeeId}) AND ActionType is null AND b.IsActive=1 ORDER BY a.DutyDate desc";
        manualPunchesVM.ManualPunchesAttendanceVMList = (await _unitOfWork.ManualPunches.GetTableData<ManualPunchesAttendanceViewModel>(sQuery));
        return manualPunchesVM;
    }

    public async Task<AttendanceHistoryViewModel> SendManualPunchesforApproval(AttendanceHistoryViewModel inputData)
    {
        string returnMsg = string.Empty;
        List<AttendanceHistory> attendacneHistoryList = new List<AttendanceHistory>();
        attendacneHistoryList = _mapper.Map<List<AttendanceHistory>>(inputData.AttendanceList);
        attendacneHistoryList.ForEach(x => { x.UnitId = inputData.UnitId; });

        returnMsg = await _unitOfWork.ManualPunches.SendManualPunchesforApproval(attendacneHistoryList, inputData.Profile, inputData.DisplayName, inputData.EmailProvider);
        inputData.DisplayMessage = returnMsg;
        return inputData;
    }

    public async Task<dynamic> CallSPUsingDapper()
    {
        string returnMsg = string.Empty;

        var param = new DynamicParameters();
        param.Add("@UnitIds", 0);
        param.Add("@EmployeeId", 152);
        var data = _unitOfWork.ManualPunches.GetAttendance("GetAttendance", param);
        //data = await _unitOfWork.ManualPunches.GetSPData("GetAttendance", param);
        //var data = _unitOfWork.ManualPunches.GetSPData<AttendanceHistory>("GetAttendance", param);
        //var data= _unitOfWork.ManualPunches.CallProcedureAsync("GetAttendance", input, output, expression);
        return data;
    }

    [HttpPost]
    public async Task<string> ManualPunchesProcessing(ManualPunchesAction userAction)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"ManualPunchesIds", userAction.ManualPunchIds, DbType.String);
            parms.Add(@"@ActionType", userAction.ActionType, DbType.String);
            parms.Add(@"@ActionRemarks", userAction.ActionRemarks, DbType.String);
            try
            {
                await _unitOfWork.ManualPunches.GetStoredProcedure("ManualPunchesProcessing", parms);
            }
            catch (Exception ex) { return "Error while saving punch response."; }
            try
            {
                string returnMessage = await _unitOfWork.ManualPunches.SendManualPunchActionMail(userAction);

                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ManualPunchesProcessing)}");
            throw;
        }
    }

    [HttpGet]
    [Route("/EmployeeAttendance/ManualPunchesProcessing/{Ids}&{ua}")]
    public async Task<string> ManualPunchesProcessing(string Ids, string ua)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"ManualPunchesIds", Ids, DbType.String);
            parms.Add(@"@ActionType", ua, DbType.String);
            _unitOfWork.ManualPunches.GetStoredProcedure("ManualPunchesProcessing", parms);
            return "Success";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ManualPunchesProcessing)}");
            throw;
        }
    }
    public async Task<String> SaveEmployeeShiftDetails(AttendanceHistoryViewModel attendanceVM)
    {
        List<AttendanceHistory> attendanceList = new List<AttendanceHistory>();
        attendanceList = _mapper.Map<List<AttendanceHistory>>(attendanceVM.AttendanceHistoryList);
        attendanceVM.DisplayMessage = await _unitOfWork.AttendanceHistory.SaveEmployeeShiftDetails(attendanceList, attendanceVM.UnitId);
        return attendanceVM.DisplayMessage;
    }
    public AttendanceHistoryDTO GetEmployeeShiftDetails(AttendanceHistoryDTO dto)
    {
        try
        {
            List<AttendanceHistory> attendanceList = new List<AttendanceHistory>();
            //AttendanceHistoryDTO attendanceHistoryDTO = _mapper.Map<AttendanceHistoryDTO>(_unitOfWork.AttendanceHistory.GetFilter(x => x.EmployeeId == dto.EmployeeId && x.DutyDate == dto.DutyDate));
            var a = _unitOfWork.AttendanceHistory.GetFilter(x => x.EmployeeId == dto.EmployeeId && x.DutyDate == dto.DutyDate).Result;
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public AttendanceHistoryViewModel MarkFaceAttendance(int employeeId, DateTime attendanceTime, string UpdateTime, string TicketId,bool autoClockedOut=false)
    {
        AttendanceHistoryViewModel ahVM = new AttendanceHistoryViewModel();
        var paramData = new DynamicParameters();
        paramData.Add("EmployeeId", employeeId, DbType.Int32);
        paramData.Add("AttendanceTime", attendanceTime.ToString("yyyy-MM-dd HH:mm:ss"), DbType.DateTime);
        paramData.Add("UpdateTime", UpdateTime, DbType.String);
        paramData.Add("TicketId", TicketId, DbType.String);
        paramData.Add("IsAutoClockedOut", autoClockedOut, DbType.String);
        ahVM.FaceRecognitionAttendanceList = _mapper.Map<List<FaceAttendanceDTO>>((_unitOfWork.FaceAttendance.GetSPData("MarkFaceAttendance", paramData).Result));
        ahVM.DisplayMessage = "Success";
        return ahVM;
    }

    public async Task<AttendanceHistoryViewModel> GetAttendanceInfo(int employeeId, DateTime dutyDate)
    {
        AttendanceHistoryViewModel ahVM = new AttendanceHistoryViewModel();
        string attndDate = dutyDate.ToString("yyyy-MM-dd");

        var paramData = new DynamicParameters();
        paramData.Add("EmployeeId", employeeId, DbType.Int32);
        paramData.Add("DutyDate", dutyDate.ToString("yyyy-MM-dd HH:mm:ss"), DbType.DateTime);
        paramData.Add("TicketId", "", DbType.String);
        ahVM = (await _unitOfWork.FaceAttendance.GetSPData<AttendanceHistoryViewModel>("GetFaceAttendanceStayHrours", paramData)).FirstOrDefault();
        
        if (ahVM != null)
        {
            if (ahVM.Status != null)
            {
                if (ahVM.Status.ToUpper() == "A")
                    ahVM.Status = "Absent";
                else if (ahVM.Status.ToUpper() == "P")
                    ahVM.Status = "Present";
                else if (ahVM.Status.ToUpper() == "P/H")
                    ahVM.Status = "Present/Half Day";
            }
            else
                ahVM.Status = "Absent";
        }
        else
            ahVM = new AttendanceHistoryViewModel();


        string sQry = $@"SELECT a.[EmployeeId],[DutyDate],a.[ClockInTime],a.[ClockOutTime],a.[IsAutoClockedOut],a.[IsShiftOn],CONVERT(time(0), DATEADD(SECOND,(DATEDIFF(SECOND,ClockInTime,ClockOutTime)), 0)) StayDuration
			FROM [dbo].[FaceAttendance] a INNER JOIN EmployeeMaster b ON a.EmployeeId=b.EmployeeId 
			WHERE  b.isActive=1 AND b.EmployeeId={employeeId} AND DutyDate = '{attndDate}' ORDER BY a.[ClockInTime] DESC";
        //ahVM.AttendanceHistory = _mapper.Map<AttendanceHistoryDTO>(_unitOfWork.AttendanceHistory.GetTableData<AttendanceHistory>(sQry).Result.FirstOrDefault());
        ahVM.FaceRecognitionAttendanceList = _mapper.Map<List<FaceAttendanceDTO>>(_unitOfWork.AttendanceHistory.GetTableData<FaceAttendanceDTO>(sQry).Result);
        ahVM.FaceRecognitionAttendanceList.ForEach(x =>
        {
            x.sClockInTime = x.ClockInTime == null ? "" : x.ClockInTime.Value.ToString("HH:mm") + ""; x.sClockOutTime = x.ClockOutTime == null ? "" : x.ClockOutTime.Value.ToString("HH:mm") + "";
        });

        ahVM.sFaceAttendanceStayHours = ahVM.FaceAttendanceStayHours == null ? "" : ((ahVM.FaceAttendanceStayHours.Value.Hour> 1 ? "hours " : "hour ") + $"<span class='fw-bold'>{ahVM.FaceAttendanceStayHours.Value.ToString("HH:mm")}</span>");
        //ahVM.sEarlyDelayTime = ahVM.EarlyDelayTime == null ? "" : (ahVM.EarlyDelayTime.Value.ToString("HH:mm") + (ahVM.EarlyDelayTime.Value.Hour > 1 ? " hours" : " hour"));
        TimeSpan? earlyLate = TimeSpan.FromSeconds(ahVM.EarlyDelayTime!=null && Math.Abs(ahVM.EarlyDelayTime.Value)>0 ? Math.Abs(ahVM.EarlyDelayTime.Value):0);
        DateTime? timeEarlyLate = DateTime.Today.Add(earlyLate.Value);
        if ((!earlyLate.ToString().Equals("00:00:00")) && ahVM.EarlyDelayTime.Value > 0)
            ahVM.sEarlyDelayTime = earlyLate == null ? "" : (timeEarlyLate.Value.ToString("HH:mm") + (earlyLate.Value.Hours > 1 ? " hours early " : " hour early "));
        else if ((!earlyLate.ToString().Equals("00:00:00")) && ahVM.EarlyDelayTime.Value < 0)
        {
            ahVM.sEarlyDelayTime = earlyLate == null ? "" : (timeEarlyLate.Value.ToString("HH:mm") + (Math.Abs(earlyLate.Value.Hours) > 1 ? " hours late " : " hour late "));
        }
        else
            ahVM.sEarlyDelayTime = "";
        if (!(ahVM.LastFaceLogin == null || ahVM.LastFaceLogin.ToString().Equals("01-01-0001 00:00:00")))
        {
            ahVM.sInTime = ahVM.LastFaceLogin == null ? "" : ahVM.LastFaceLogin.Value.ToString("HH:mm");   //$"{Hr}:{Min}";
        }
        else
        {
            ahVM.sInTime = ahVM.InTime == null ? "" : ahVM.InTime.Value.ToString("HH:mm"); //$"{Hr}:{Min}";
            ahVM.sOutTime = ahVM.OutTime == null ? "" : ahVM.OutTime.Value.ToString("HH:mm"); //$"{Hr1}:{Min1}";
        }
        ahVM.DisplayMessage = "Success";
        return ahVM;
    }

    public AttendanceHistoryViewModel UpdateGPSInAttendance(AttendanceHistoryViewModel inputData, DateTime attendanceTime)
    {
        AttendanceHistoryViewModel ahVM = new AttendanceHistoryViewModel();
        var paramData = new DynamicParameters();
        paramData.Add("EmployeeId", inputData.EmployeeId, DbType.Int32);
        paramData.Add("AttendanceTime", attendanceTime.ToString("yyyy-MM-dd HH:mm:ss"), DbType.DateTime);
        paramData.Add("HostName", inputData.HostName, DbType.String);
        paramData.Add("IPAddress", inputData.IPAddress, DbType.String);
        paramData.Add("GPSLocation", inputData.GPSLocation, DbType.String);
        paramData.Add("longitude", inputData.longitude, DbType.String);
        paramData.Add("latitude", inputData.latitude, DbType.String);
        paramData.Add("AttendanceType", inputData.AttendanceType, DbType.String);

        ahVM.FaceRecognitionAttendanceList = _mapper.Map<List<FaceAttendanceDTO>>((_unitOfWork.FaceAttendance.GetSPData("UpdateGPSInAttendance", paramData).Result));
        ahVM.DisplayMessage = "Success";
        return ahVM;
    }
    

    public async Task<IActionResult> GetEmployeeEditTicketList(int clientId, int UnitId)
    {
        try
        {
            string sQuery = "";

            sQuery = @"select ee.TicketID TicketId,ee.EmployeeId
                            ,(select EmployeeName from EmployeeMaster em where em.EmployeeId=ee.EmployeeId)EmployeeName
                            ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ee.EmployeeId)Department
                            from 
                            EditEmployeeData ee
                            join 
                            EmployeeMaster em
							on em.EmployeeId=ee.EmployeeId
							join 
							EmployeeValidationMaster evm
							on ee.EmployeeValidationId=evm.EmployeeValidationId
                            where em.ClientId=" + clientId + " and em.UnitId=" + UnitId + " and isnull(IsApproved,0)=0  Group by ee.TicketID,ee.EmployeeId union select TicketId "
                            + " ,ad.EmployeeId "
                            + " ,(select EmployeeName from EmployeeMaster em where em.employeeid=ad.EmployeeId)EmployeeName"
                            + " ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ad.EmployeeId)Department"
                            + " from [dbo].[AddDeleteTableAction] ad where IsActive=1 and ActionStatus=0  and EmployeeId in (select EmployeeId from EmployeeMaster where ClientId=" + clientId + " and UnitId=" + UnitId + ") Group by" + " ad.TicketID,ad.EmployeeId";

            List<EmployeeEditTicketViewModel> dto = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeEditTicketViewModel>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }
}