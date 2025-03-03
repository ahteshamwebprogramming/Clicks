using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using static Azure.Core.HttpHeader;

namespace SimpliHR.Endpoints;

[ApiController]
[Route("[controller]")]
public class AttendanceRosterController : ControllerBase
{

    private readonly DapperDBContext _dapperDBContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AttendanceRosterController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;


    public AttendanceRosterController(IUnitOfWork unitOfWork, ILogger<AttendanceRosterController> logger, IMapper mapper,DapperDBContext dapperDBContext, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBContext = dapperDBContext;
        _configuration = configuration;
    }

    public async Task<IList<AttendanceRosterDTO>> AttendanceRosters(int limit, int offset, string sWhere = "", string sOrderBy = "")
    {
        try
        {
            IList<AttendanceRosterDTO> outputModel = new List<AttendanceRosterDTO>();
            outputModel = _mapper.Map<IList<AttendanceRosterDTO>>(await _unitOfWork.AttendanceRoster.GetAllPagedAsync(limit, offset, sWhere, sOrderBy));
            return outputModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AttendanceRosters)}");
            throw;
        }
    }

    [HttpGet]
    public async Task<string> DeleteRosterDetails(int rosterId)
    {
        string sReturnMsg=string.Empty;
        if (rosterId!=0)
            sReturnMsg = await _unitOfWork.AttendanceRoster.DeleteRosterDetails(rosterId);

        return sReturnMsg;
    }


    [HttpGet]
    public async Task<AttendanceRosterDTO> GetRoster(int rosterId)
    {
        try
        {
            AttendanceRosterDTO outputModel = new AttendanceRosterDTO();
            outputModel = _mapper.Map<AttendanceRosterDTO>(await _unitOfWork.AttendanceRoster.FindByIdAsync(rosterId));
            string query = $"select distinct employeeId from attendancehistory where EntrySourceId={rosterId} and EntrySource='R' order by EmployeeId asc";
            var employeeList = await _unitOfWork.AttendanceHistory.GetQueryAll(query);
            outputModel.RosterEmployeeIDs = string.Join(",", employeeList.Select(t => { return t.EmployeeId; }));
            return outputModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AttendanceRosters)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAttendanceRoster(AttendanceViewModel inputVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                IList<AttendanceRosterDTO> outputModel = new List<AttendanceRosterDTO>();
                AttendanceRoster inputData = new AttendanceRoster();
                inputData = _mapper.Map<AttendanceRoster>(inputVM.AttendanceRoster);
                List<EmployeeKeyValues> rosterEmployees = new List<EmployeeKeyValues>();
                string returnMsg = "";
                Expression<Func<AttendanceRoster, bool>> expression = null;
                if (inputVM.AttendanceRoster.RosterId!=0)
                    expression = a => a.RosterId != inputVM.AttendanceRoster.RosterId && a.UnitId == inputData.UnitId && a.DepartmentId == inputData.DepartmentId && a.RosterName == inputData.RosterName && a.IsActive == true;
                else
                    expression = a => a.UnitId == inputData.UnitId && a.DepartmentId==inputData.DepartmentId && a.RosterName == inputData.RosterName && a.IsActive == true;
                if (! (await _unitOfWork.AttendanceRoster.Exists(expression)))
                {
                    string sQry = string.Empty;
                    object param=null;
                    if (!(inputData.DepartmentId == null || inputData.DepartmentId == 0))
                    {
                        if (inputData.EmployeesSelection == 1)
                            rosterEmployees = inputVM.AttendanceMastersKeyValues.EmployeeKeyValues.Where(r => r.DepartmentId == inputData.DepartmentId).ToList();
                        else
                            rosterEmployees = inputVM.AttendanceMastersKeyValues.EmployeeKeyValues.Where(r => inputVM.AttendanceRoster.RosterEmployeeIDs.Split(",").Contains(r.EmployeeId.ToString())).ToList();
                    }
                    else if (inputVM.AttendanceRoster.RosterEmployeeIDs!=null && inputVM.AttendanceRoster.RosterEmployeeIDs != "")
                    {
                        sQry = "SELECT a.EmployeeId,a.LastWorkingDateAdmin ExitDate FROM  EmployeeExitResignation a WHERE AdminApproval=1 AND EmployeeId in (@empIds)";
                        param = new { @empIds = inputVM.AttendanceRoster.RosterEmployeeIDs };
                        rosterEmployees = inputVM.AttendanceMastersKeyValues.EmployeeKeyValues.Where(r => inputVM.AttendanceRoster.RosterEmployeeIDs.Split(",").Contains(r.EmployeeId.ToString())).ToList();
                    }
                    else
                    {
                        sQry = "SELECT a.EmployeeId,a.LastWorkingDateAdmin ExitDate FROM  EmployeeExitResignation a WHERE AdminApproval=1 AND EmployeeId in (Select EmployeeIds FROM EmployeeMaster WHERE UnitId=@unitId)";
                        param = new { @unitId = inputData.UnitId };
                        rosterEmployees = inputVM.AttendanceMastersKeyValues.EmployeeKeyValues.Where(r => r.UnitId == inputData.UnitId).ToList();
                    }

                    List<EmployeeKeyValues> employeeExitList = new List<EmployeeKeyValues>();
                    employeeExitList = await _unitOfWork.EmployeeExitResignation.ExecuteQuery<EmployeeKeyValues>(sQry, param);
                    rosterEmployees.ForEach(p => {
                        var exitData = employeeExitList.Where(r => r.EmployeeId == p.EmployeeId).FirstOrDefault();
                        p.DOJ = _unitOfWork.EmployeeMaster.FindFirstByExpression(r => r.EmployeeId == p.EmployeeId).Doj;
                        p.ExitDate = exitData==null ? null:exitData.ExitDate;
                           
                    });
                    

                    returnMsg = await _unitOfWork.AttendanceRoster.SaveAttendanceShiftRoster(inputData, rosterEmployees, inputData.UnitId);

                }
                else
                    returnMsg = "Roster name is already exist for selected unit and department";

                return Ok(returnMsg);

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AttendanceRosters)}");
            throw;
        }
    }
    //public async Task<IActionResult> UpdateAttendanceRoster(AttendanceRosterDTO inputDTO)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            IList<AttendanceRosterDTO> outputModel = new List<AttendanceRosterDTO>();
    //            AttendanceRoster inputVM = new AttendanceRoster();
    //            inputVM = _mapper.Map<AttendanceRoster>(inputDTO);
    //            await _unitOfWork.AttendanceRoster.UpdateAsync(inputVM);
    //            //outputModel = _mapper.Map<IList<AttendanceRosterDTO>>(_unitOfWork.AttendanceRoster.GetQueryAll("select * from AttendanceRoster"));
    //            return Ok("Success");
    //        }
    //        return Ok("Invalid Model");
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Attendance {nameof(AttendanceRosters)}");
    //        throw;
    //    }
    //}


}