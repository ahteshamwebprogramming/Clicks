using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Leave;

using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Services.DBContext;
using SimpliHR.Core.Helper;
using Dapper;
using SimpliHR.Infrastructure.Models.Attendance;
using System.Data;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Net.Mail;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Endpoints.Leave;

[Route("api/[controller]/[action]")]
[ApiController]
public class LeaveAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LeaveAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBContext _dapperDBContext;

    public LeaveAPIController(IUnitOfWork unitOfWork, ILogger<LeaveAPIController> logger, IMapper mapper, DapperDBContext dapperDBContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBContext = dapperDBContext;
    }

    [HttpPost]
    public async Task<IActionResult> GetLeaveYear(LeaveCalenderYearDTO inputDTO)
    {
        try
        {
            LeaveCalenderYearDTO outputDTO = _mapper.Map<LeaveCalenderYearDTO>(await _unitOfWork.LeaveCalenderYear.GetByIdAsync(inputDTO.LeaveYearId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetLeaveYear)}");
            throw;
        }
    }

    [HttpPost(Name = "GetLeaveYearList")]
    public async Task<IActionResult> GetLeaveYearList(Core.Helper.RequestParams requestParams, int? UnitId)
    {

        try
        {
            var returnData = _mapper.Map<IList<LeaveCalenderYearDTO>>(await _unitOfWork.LeaveCalenderYear.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.UnitId == UnitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.CalendarName))));
            IList<LeaveCalenderYearDTO> outputModel = new List<LeaveCalenderYearDTO>();

            outputModel = returnData.Select(r => new LeaveCalenderYearDTO
            {
                LeaveYearId = r.LeaveYearId,
                CalendarName = r.CalendarName,
                StartDate = r.StartDate,
                EndDate = r.EndDate

            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetLeaveYearList)}");
            throw;
        }

    }


    [HttpPost]
    public IActionResult SaveLeaveYear(LeaveCalenderYearDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.LeaveYearId == 0)
                {
                    Expression<Func<LeaveCalenderYear, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.CalendarName == inputDTO.CalendarName && a.IsActive == true;
                    if (!_unitOfWork.LeaveCalenderYear.Exists(expression))
                    {
                        _unitOfWork.LeaveCalenderYear.AddAsync(_mapper.Map<LeaveCalenderYear>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate year leave found");
                }
                else
                {
                    // Expression<Func<AttendanceSetting, bool>> expression = a => a.LegendType == inputDTO.LegendType && a.AttendanceSettingId != inputDTO.AttendanceSettingId && a.IsActive == true;
                    // if (!_unitOfWork.AttendanceSetting.Exists(expression))
                    // {
                    _unitOfWork.LeaveCalenderYear.Update(_mapper.Map<LeaveCalenderYear>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    // }
                    // else
                    //  return BadRequest("Duplicate Attendance Setting found");

                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveLeaveYear)}");
            throw;
        }
    }



    [HttpPost]
    public IActionResult SaveLeaveAttribute(LeaveAttributeDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.LeaveAttributeId == 0)
                {
                    Expression<Func<LeaveAttribute, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.LeaveTypeId == inputDTO.LeaveTypeId && a.CalenderYearId == inputDTO.CalenderYearId;
                    if (!_unitOfWork.LeaveAttribute.Exists(expression))
                    {
                        _unitOfWork.LeaveAttribute.AddAsync(_mapper.Map<LeaveAttribute>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate leave type found");
                }
                else
                {
                    _unitOfWork.LeaveCalenderYear.Update(_mapper.Map<LeaveCalenderYear>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveLeaveAttribute)}");
            throw;
        }
    }

    [HttpPost(Name = "GetLeaveAttributes")]
    public async Task<IActionResult> GetLeaveAttributes(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<LeaveAttributeDTO> ViewModel = new List<LeaveAttributeDTO>();
            ViewModel = _mapper.Map<IList<LeaveAttributeDTO>>(await _unitOfWork.LeaveAttribute.GetPagedListWithExpression(requestParams, p => p.IsActive == true && p.UnitId == UnitId));

            return Ok(ViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetLeaveAttributes)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLeaveAttribute(LeaveAttributeDTO leaveAttributeDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LeaveAttribute leaveAttribute = _mapper.Map<LeaveAttribute>(await _unitOfWork.LeaveAttribute.GetByIdAsync(leaveAttributeDTO.LeaveAttributeId));
                leaveAttribute.IsActive = false;
                _unitOfWork.LeaveAttribute.Update(leaveAttribute);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteLeaveAttribute)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateLeaveAttribute(LeaveAttributeDTO leaveAttributeDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<LeaveAttribute, bool>> expression = a => a.CountryName.Trim().Replace(" ", "") == countryDTO.CountryName.Trim().Replace(" ", "") && a.CountryId != countryDTO.CountryId && a.IsActive == true;
                //if (!_unitOfWork.CountryMaster.Exists(expression))
                //{
                var leaveAttributeViewModel = _mapper.Map<LeaveAttribute>(leaveAttributeDTO);
                _unitOfWork.LeaveAttribute.Update(leaveAttributeViewModel);
                _unitOfWork.Save();
                return Ok("Success");
                //}
                //else
                //return BadRequest("Duplicate Country found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Leave Attribute updates {nameof(UpdateLeaveAttribute)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetLeaveAttribute(LeaveAttributeDTO inputDTO)
    {
        try
        {
            LeaveAttributeDTO outputDTO = _mapper.Map<LeaveAttributeDTO>(await _unitOfWork.LeaveAttribute.GetByIdAsync(inputDTO.LeaveAttributeId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving employee {nameof(GetLeaveAttribute)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteLeaveYear(LeaveCalenderYearDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LeaveCalenderYear outputMaster = _mapper.Map<LeaveCalenderYear>(await _unitOfWork.LeaveCalenderYear.GetByIdAsync(inputDTO.LeaveYearId));
                outputMaster.IsActive = false;
                _unitOfWork.LeaveCalenderYear.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Leave Type {nameof(DeleteLeaveYear)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<string> SaveEmployeeLeaveDetails(EmployeeLeaveDetailsDTO inputDTO)
    {
        try
        {

            //var status = SendMail(inputDTO);

            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeId", inputDTO.EmployeeId, DbType.Int32);
            parms.Add(@"@LeaveTypeId", inputDTO.LeaveTypeId, DbType.Int32);
            parms.Add(@"@UnitId", inputDTO.UnitId, DbType.Int32);
            parms.Add(@"@StartDate", inputDTO.StartDate, DbType.Date);
            parms.Add(@"@EndDate", inputDTO.EndDate, DbType.Date);
            parms.Add(@"@NoOfLeave", inputDTO.NoOfLeave, DbType.Decimal);
            parms.Add(@"@Remarks", inputDTO.Remarks, DbType.String);
            parms.Add(@"@BillRequired", inputDTO.IsBillRequired, DbType.Boolean);
            parms.Add(@"@BillName", inputDTO.BillName, DbType.String);
            parms.Add(@"@TicketId", inputDTO.TicketId, DbType.String);
            parms.Add(@"@CreatedBy", inputDTO.CreatedBy, DbType.String);

            try
            {

                var result = await _unitOfWork.EmployeeLeaveDetails.GetSPData("usp_ApplyEmployeeLeave", parms);

                //  var status = await SendMail(inputDTO);
                string returnMessage = "";
                //var result = await _unitOfWork.EmployeeLeaveDetails.GetStoredProcedure("usp_ApplyEmployeeLeave", parms);
                if (result.Count > 0)
                {
                    if (Convert.ToInt32(result[0].Remarks) == 0)
                    {
                        var status = await SendMail(inputDTO);
                        returnMessage = "Leave applied Successfully";
                        // return returnMessage;
                    }
                    else if (Convert.ToInt32(result[0].Remarks) == 90)
                    {
                        returnMessage = "Duplicate leave/s found";
                        // return returnMessage;
                    }
                    else if (Convert.ToInt32(result[0].Remarks) == 99)
                    {
                        returnMessage = "Records not found!,contact admin";
                        // return returnMessage;
                    }
                    else if (Convert.ToInt32(result[0].Remarks) == 10)
                    {
                        returnMessage = "You are not applicable to continue applying multiple leave/s";

                    }
                    else
                    {
                        returnMessage = "Failed!,Contact admin";
                    }
                }
                return returnMessage;
            }
            catch (Exception ex) { return "Error while leave applying."; }

            //return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeLeaveDetails)}");
            throw;
        }
    }

    async Task<bool> SendMail(EmployeeLeaveDetailsDTO inputDTO)
    {
        try
        {

            //string folderpath = Path.Combine(inputDTO.Profile, "EmployeePofile");
            //string path = Path.Combine(folderpath, Convert.ToString(inputDTO.UnitId));
            string path = inputDTO.Profile;
            LeaveAction objLeaveAction = new LeaveAction();
            objLeaveAction.TicketId = inputDTO.TicketId;
            objLeaveAction.EmployeeId = inputDTO.EmployeeId;
            objLeaveAction.Profile = path;
            objLeaveAction.LeaveIds = Convert.ToString(inputDTO.LeaveTypeId);
            objLeaveAction.DisplayName = inputDTO.DisplayName;
            objLeaveAction.EmailProvider = inputDTO.EmailProvider;
            string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveApprovalMail(objLeaveAction);
            return true;

            // string returnMessage = "The leave applied has successfully";// await _unitOfWork.ManualPunches.SendManualPunchActionMail(inputDTO);

            //  return returnMessage;

        }
        catch (Exception ex) { return false; }
    }

    public async Task<EmployeeLeaveDetailsDTO> GetLeavePendingForApproval(int employeeId, int? unitId, int? isAdmin)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        //string sQuery = $"select a.*,l.LeaveType,C.EmployeeName,C.EmployeeCode,w.[location] from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId " +
        //     $"LEFT JOIN EmployeeMaster c ON a.EmployeeId=c.EmployeeId inner join WorkLocationMaster w on C.worklocationid =w.worklocationid " +
        //     $"WHERE a.EmployeeId IN (SELECT EmployeeId FROM EmployeeMaster WHERE ManagerId={employeeId}  or  HODId={employeeId} ) AND  a.LeaveStatus in (1,90) ORDER BY c.EmployeeName\r\n";
        //EmployeeLeaveList.EmployeeAppliedList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveDetailsDTO>(sQuery));

        var parms = new DynamicParameters();
        parms.Add(@"@UnitId", unitId, DbType.Int32);
        parms.Add(@"@EmployeeId", employeeId, DbType.Int32);
        parms.Add(@"@IsAdmin", isAdmin, DbType.Int32);
        //string sQuery = $"select  a.*,details.LeaveType,details.LeaveTypeCode from EmployeeLeaveHistory a inner join [EmployeeLeaveDetails] l on a.ticketId=l.ticketId " +
        //    $"inner join [LeaveTypeMaster] details on a.LeaveTypeId=details.LeaveTypeId " +
        //    $"where   a.ticketId={ticketId} and a.LeaveStatus in (0,1) ORDER BY a.CreatedOn desc\r\n";
        EmployeeLeaveList.EmployeeAppliedList = (await _unitOfWork.EmployeeLeaveDetails.GetSPData<EmployeeLeaveDetailsDTO>("usp_getRegulaizeLeaves", parms));

        return EmployeeLeaveList;
    }

    public async Task<EmployeeLeaveDetailsDTO> GetLeaveforReversal(int leaveTypeId, int employeeId, bool isEmployee)
    {
        string sQuery = "", sQuery1;
        // int leaveTypeId = 0;
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        if (isEmployee == false)
        {
            sQuery = $"select a.*,l.LeaveType,l.LeaveTypeCode,C.EmployeeName from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId " +
               $"LEFT JOIN EmployeeMaster c ON a.EmployeeId=c.EmployeeId  WHERE a.EmployeeId IN (SELECT EmployeeId FROM EmployeeMaster WHERE ManagerId={employeeId}) AND  a.LeaveStatus in (0) AND (startdate>= getdate() or enddate> getdate()) ORDER BY c.EmployeeName\r\n";
        }
        else
        {
            sQuery = $"select a.*,l.LeaveType,l.LeaveTypeCode,C.EmployeeName from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId " +
                           $"LEFT JOIN EmployeeMaster c ON a.EmployeeId=c.EmployeeId  WHERE a.EmployeeId = {employeeId} AND  a.LeaveStatus in (0) AND (startdate>= getdate() or enddate> getdate()) ORDER BY c.EmployeeName\r\n";
        }

        if (leaveTypeId == 0)
            sQuery1 = $"select H.*, L.LeaveType  from EmployeeLeaveHistory H left join LeaveTypeMaster L on H.Leavetypeid=L.LeaveTypeId " +
               $"where  H.EmployeeId={employeeId} ORDER BY H.CreatedOn desc \r\n";
        else
            sQuery1 = $"select H.*, L.LeaveType  from EmployeeLeaveHistory H left join LeaveTypeMaster L on H.Leavetypeid=L.LeaveTypeId " +
           $"where  H.EmployeeId={employeeId} and H.Leavetypeid={leaveTypeId} ORDER BY H.CreatedOn desc\r\n";

        EmployeeLeaveList.EmployeeLeaveHistoryList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveHistoryDTO>(sQuery1));
        EmployeeLeaveList.EmployeeRevarsalList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveDetailsDTO>(sQuery));
        return EmployeeLeaveList;
    }

    //[HttpPost]
    //public async Task<IActionResult> GetLeaveDetailsbyId(EmployeeLeaveDetailsDTO inputDTO)
    //{
    //    try
    //    {
    //        EmployeeLeaveDetailsDTO outputDTO = _mapper.Map<EmployeeLeaveDetailsDTO>(await _unitOfWork.EmployeeLeaveDetails.GetByIdAsync(inputDTO.LeaveDetailsId));
    //        HttpResponseMessage httpMessage = new HttpResponseMessage();
    //        if (outputDTO == null)
    //        {
    //            httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
    //            outputDTO = CommonHelper.GetClassObject(outputDTO);
    //        }
    //        else
    //            httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, true);

    //        outputDTO.HttpMessage = httpMessage;
    //        return Ok(outputDTO);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Employee Leave Details  {nameof(GetLeaveDetailsbyId)}");
    //        throw;
    //    }
    //}


    [HttpPost]
    public decimal? GetLeaveBalance11(EmployeeLeaveBalanceDTO inputDTO)
    {
        try
        {
            decimal? LeaveBalance = 0;
            // decimal? LeaveBalance = _unitOfWork.EmployeeLeaveBalance.FindFirstByExpression(p => p.LeaveTypeId == inputDTO.LeaveTypeId && p.EmployeeId == inputDTO.EmployeeId).LeaveBalance;

            return LeaveBalance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee balance {nameof(GetLeaveBalance)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetLeaveBalance(LeaveAttributeDTO inputDTO, string employeeId)
    {
        try
        {
            decimal? LeaveBalance = 0;
            LeaveAttributeDTO outputDTO = new LeaveAttributeDTO();
            int empID = Convert.ToInt32(employeeId);

            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeId", empID, DbType.Int32);
            parms.Add(@"@LeaveTypeId", inputDTO.LeaveTypeId, DbType.Int32);

            IList<EmployeeLeaveBalanceDTO> outputModel = new List<EmployeeLeaveBalanceDTO>();
            outputModel = _mapper.Map<IList<EmployeeLeaveBalanceDTO>>(await _unitOfWork.EmployeeLeaveBalance.GetSPData<EmployeeLeaveBalanceDTO>("usp_GetEmployeeBalanceByLeaveType", parms));

            //Expression<Func<EmployeeLeaveBalance, bool>> expression1 = a => a.LeaveTypeId == inputDTO.LeaveTypeId && a.EmployeeId == empID;
            //if (_unitOfWork.EmployeeLeaveBalance.Exists(expression1))
            //{
            //    LeaveBalance = _unitOfWork.EmployeeLeaveBalance.FindFirstByExpression(p => p.LeaveTypeId == inputDTO.LeaveTypeId && p.EmployeeId == empID).LeaveBalance;
            outputDTO = _mapper.Map<LeaveAttributeDTO>(_unitOfWork.LeaveAttribute.FindFirstByExpression(p => p.LeaveTypeId == inputDTO.LeaveTypeId && p.UnitId == inputDTO.UnitId));
            if (outputModel.Count > 0)
            {
                outputDTO.LeaveBalance = outputModel[0].LeaveBalance;
                outputDTO.DisplayMessage = "None";
            }
            else
            {
                outputDTO.LeaveBalance = 0;
                outputDTO.DisplayMessage = "Details not found, pls contact admin";
            }
            // LeaveAttributeDTO outputDTO = _unitOfWork.LeaveAttribute.Find(p =>p.LeaveTypeId == inputDTO.LeaveTypeId);

            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee balance {nameof(GetLeaveBalance)}");
            throw;
        }
    }




    [HttpPost(Name = "GetEmployeeLeaveSummary")]
    public async Task<IActionResult> GetEmployeeLeaveSummary(Core.Helper.RequestParams requestParams, int? EmployeeId, int? genderId)
    {

        try
        {

            //  IList<EmployeeLeaveBalanceDTO> outputResult =null;

            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeId", EmployeeId, DbType.Int32);
            parms.Add(@"@GenderId", genderId, DbType.Int32);

            IList<EmployeeLeaveBalanceDTO> outputModel = new List<EmployeeLeaveBalanceDTO>();
            outputModel = _mapper.Map<IList<EmployeeLeaveBalanceDTO>>(await _unitOfWork.EmployeeLeaveBalance.GetSPData<EmployeeLeaveBalanceDTO>("usp_GetEmployeeBalance", parms));


            //if (genderId == 1)
            //{
            //    outputResult = _mapper.Map<IList<EmployeeLeaveBalanceDTO>>(await _unitOfWork.EmployeeLeaveBalance.GetAll(requestParams: requestParams,
            //                                                                                    expression: (p => p.EmployeeId == EmployeeId && p.LeaveTypeCode != "MTL" && p.LeaveTypeCode != "MT" && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
            //                                                                                    orderBy: (m => m.OrderBy(x => x.LeaveTypeId))));
            //}
            //else if (genderId == 1)
            //{
            //    outputResult = _mapper.Map<IList<EmployeeLeaveBalanceDTO>>(await _unitOfWork.EmployeeLeaveBalance.GetAll(requestParams: requestParams,
            //                                                                                              expression: (p => p.EmployeeId == EmployeeId && p.LeaveTypeCode != "PTL" && p.LeaveTypeCode != "PT" && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
            //                                                                                              orderBy: (m => m.OrderBy(x => x.LeaveTypeId))));
            //}
            //else
            //{
            //    outputResult = _mapper.Map<IList<EmployeeLeaveBalanceDTO>>(await _unitOfWork.EmployeeLeaveBalance.GetAll(requestParams: requestParams,
            //                                                                                              expression: (p => p.EmployeeId == EmployeeId  && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
            //                                                                                              orderBy: (m => m.OrderBy(x => x.LeaveTypeId))));
            //}
            //IList<EmployeeLeaveBalanceDTO> outputModel = new List<EmployeeLeaveBalanceDTO>();

            //outputModel = outputResult.Select(r => new EmployeeLeaveBalanceDTO
            //{
            //    LeaveBalance = r.LeaveBalance,
            //   LeaveType = r.LeaveTypes.LeaveType,
            //    LeaveTypeId = r.LeaveTypeId,
            //    LeaveTypeCode = r.LeaveTypeCode,
            //    OpeningBalance = r.OpeningBalance,
            //    TotalApplied = r.TotalApplied,
            //    TotalAvailed = r.TotalAvailed

            //}).ToList();

            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetEmployeeLeaveSummary)}");
            throw;
        }

    }


    public async Task<EmployeeLeaveDetailsDTO> GetEmployeeLeaveDetails(int leaveTypeId,int employeeId)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        string sQuery = $"select a.*,l.LeaveType,l.LeaveTypeCode from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId " +
            $"where  a.LeaveTypeId={leaveTypeId} and a.employeeid={employeeId} and a.LeaveStatus =0 ORDER BY a.CreatedOn desc\r\n";
        EmployeeLeaveList.EmployeeLeaveList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveDetailsDTO>(sQuery));
        return EmployeeLeaveList;
    }
    public async Task<EmployeeLeaveDetailsDTO> GetEmployeeTicketDetails(string ticketId, string moduleId)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        var parms = new DynamicParameters();
        parms.Add(@"@ticketId", ticketId, DbType.String);
        parms.Add(@"@ModuleId", Convert.ToInt32(moduleId), DbType.Int32);
        //string sQuery = $"select  a.*,details.LeaveType,details.LeaveTypeCode from EmployeeLeaveHistory a inner join [EmployeeLeaveDetails] l on a.ticketId=l.ticketId " +
        //    $"inner join [LeaveTypeMaster] details on a.LeaveTypeId=details.LeaveTypeId " +
        //    $"where   a.ticketId={ticketId} and a.LeaveStatus in (0,1) ORDER BY a.CreatedOn desc\r\n";
        EmployeeLeaveList.EmployeeLeaveList = (await _unitOfWork.EmployeeLeaveDetails.GetSPData<EmployeeLeaveDetailsDTO>("usp_GetTicketHistory", parms));
        return EmployeeLeaveList;
    }

    public async Task<EmployeeLeaveDetailsDTO> GetPendingLeaveDetails(int leaveTypeId)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        string sQuery = $"select a.*,l.LeaveType,l.LeaveTypeCode from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId " +
            $"where  a.LeaveTypeId={leaveTypeId} and a.LeaveStatus=1 ORDER BY a.CreatedOn desc\r\n";
        EmployeeLeaveList.EmployeeLeaveList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveDetailsDTO>(sQuery));
        return EmployeeLeaveList;
    }

    public async Task<EmployeeLeaveDetailsDTO> GetEmployeePendingLeaveDetails(int leaveTypeId, int employeeId)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        string sQuery = $"select a.*,l.LeaveType,l.LeaveTypeCode from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId " +
            $"where  a.LeaveTypeId={leaveTypeId} and a.employeeId={employeeId} and a.LeaveStatus in (1,99) ORDER BY a.CreatedOn desc\r\n";
        EmployeeLeaveList.EmployeeLeaveList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveDetailsDTO>(sQuery));
        return EmployeeLeaveList;
    }

    [HttpPost]
    public async Task<string> LeaveRegularizeProcessing(LeaveAction userAction)
    {
        //string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveRequalizeTestMail(userAction);
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@ManualLeaveIds", userAction.LeaveIds, DbType.String);
            parms.Add(@"@ActionType", userAction.ActionType, DbType.String);
            parms.Add(@"@ActionRemarks", userAction.ActionRemarks, DbType.String);
            parms.Add(@"@ApprovedBy", userAction.ApprovedBy, DbType.String);
            try
            {
                await _unitOfWork.EmployeeLeaveDetails.GetStoredProcedure("usp_LeaveRegularizeProcessing", parms);
            }
            catch (Exception ex) { return "Error while approve/reject leave."; }
            try
            {

                string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveRequalizeMail(userAction);

                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in while approve/reject leave {nameof(LeaveRegularizeProcessing)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<string> CompOffRegularizeProcessing(CompOffAction userAction)
    {
        try
        {

            var compOffIds = new Microsoft.Data.SqlClient.SqlParameter("@CompOffIds", userAction.CompOffIds);
            var actionType = new Microsoft.Data.SqlClient.SqlParameter("@ActionType", userAction.ActionType);
            var actionRemarks = new Microsoft.Data.SqlClient.SqlParameter("@ActionRemarks", userAction.ActionRemarks);
            var approvedBy = new Microsoft.Data.SqlClient.SqlParameter("@ApprovedBy", userAction.ApprovedBy);

            try
            {
                await _unitOfWork.EmployeeCompOff.ExecuteRawQuery("EXEC usp_CompOffRegularizeProcessing @CompOffIds,@ActionType,@ActionRemarks,@ApprovedBy", new[] { compOffIds, actionType, actionRemarks, approvedBy });


                // var results=   _unitOfWork.EmployeeCompOff.GetWithRawSql("usp_LeaveRegularizeProcessing",null).ToList();
            }
            catch (Exception ex) { return "Error while approve/reject compoff."; }
            try
            {

                //  string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendCompoffRequalizeMail(userAction);
                string returnMessage = "";
                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in while approve/reject leave {nameof(LeaveRegularizeProcessing)}");
            throw;
        }
    }

    public async Task<EmployeeLeaveDetailsDTO> GetEmployeeReversalDetails(int leaveTypeId)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        string sQuery = $"select a.*,l.LeaveType,l.LeaveTypeCode from EmployeeLeaveDetails a inner join LeaveTypeMaster l on a.LeaveTypeId=l.LeaveTypeId and  (startdate>= getdate() or enddate> getdate()) " +
            $"where  a.LeaveTypeId={leaveTypeId} ORDER BY a.CreatedOn\r\n";
        EmployeeLeaveList.EmployeeRevarsalList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveDetailsDTO>(sQuery));
        return EmployeeLeaveList;
    }

    [HttpPost]
    public async Task<string> LeaveReversalProcessing(LeaveAction userAction)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@ManualLeaveIds", userAction.LeaveIds, DbType.String);
            parms.Add(@"@ActionType", userAction.ActionType, DbType.String);
            parms.Add(@"@ActionRemarks", userAction.ActionRemarks, DbType.String);
            parms.Add(@"@ApprovedBy", userAction.ApprovedBy, DbType.String);
            try
            {
                await _unitOfWork.EmployeeLeaveDetails.GetStoredProcedure("usp_LeaveReversalProcessing", parms);
            }
            catch (Exception ex) { return "Error while reversal leave."; }
            try
            {
              //  string returnMessage = "SUCCESS"; // await _unitOfWork.ManualPunches.SendApprovalMail(userAction);
                string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveReversalMail(userAction);

                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in while reversal leave {nameof(LeaveReversalProcessing)}");
            throw;
        }
    }

    public async Task<EmployeeLeaveDetailsDTO> GetEmployeeLeaveHistory(int leaveTypeId, int EmployeeId)
    {
        string sQuery = "";
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        if (leaveTypeId == 0)
            sQuery = $"select H.*, L.LeaveType  from EmployeeLeaveHistory H left join LeaveTypeMaster L on H.Leavetypeid=L.LeaveTypeId " +
               $"where  H.EmployeeId={EmployeeId} ORDER BY H.CreatedOn desc\r\n";
        else
            sQuery = $"select H.*, L.LeaveType  from EmployeeLeaveHistory H left join LeaveTypeMaster L on H.Leavetypeid=L.LeaveTypeId " +
           $"where  H.EmployeeId={EmployeeId} and H.Leavetypeid={leaveTypeId} ORDER BY H.CreatedOn desc\r\n";

        EmployeeLeaveList.EmployeeLeaveHistoryList = (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveHistoryDTO>(sQuery));
        // var result= (await _unitOfWork.EmployeeLeaveDetails.GetTableData<EmployeeLeaveHistoryDTO>(sQuery));
        return EmployeeLeaveList;
    }

    [HttpPost]
    public IActionResult SaveComboff(LeaveCompOffDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.CompOffId == 0)
                {
                    Expression<Func<LeaveCompOff, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.CalendarYear == inputDTO.CalendarYear && a.IsActive == true;
                    if (!_unitOfWork.LeaveCompOff.Exists(expression))
                    {
                        _unitOfWork.LeaveCompOff.AddAsync(_mapper.Map<LeaveCompOff>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.LeaveCompOff.Update(_mapper.Map<LeaveCompOff>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveComboff)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeComboff(EmployeeCompOffDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.CompOffId == 0)
                {
                    Expression<Func<EmployeeCompOff, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.CompOffDate == inputDTO.CompOffDate;
                    if (!_unitOfWork.EmployeeCompOff.Exists(expression))
                    {
                        _unitOfWork.EmployeeCompOff.AddAsync(_mapper.Map<EmployeeCompOff>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.EmployeeCompOff.Update(_mapper.Map<EmployeeCompOff>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveEmployeeComboff)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult RegularizEmployeeComboff(EmployeeCompOffDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.CompOffId > 0)
                {
                    EmployeeCompOffDTO existingDetails = _mapper.Map<EmployeeCompOffDTO>(_unitOfWork.EmployeeCompOff.GetAll(null, null, null).Result.Where(a => a.CompOffId == inputDTO.CompOffId && a.Status == 0).FirstOrDefault());
                    inputDTO.CreatedBy = existingDetails.CreatedBy;
                    inputDTO.CreatedOn = existingDetails.CreatedOn;
                    inputDTO.Status = 1;

                    _unitOfWork.EmployeeCompOff.Update(_mapper.Map<EmployeeCompOff>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }


            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveEmployeeComboff)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetComboff(LeaveCompOffDTO inputDTO)
    {
        try
        {
            LeaveCompOffDTO outputDTO = _mapper.Map<LeaveCompOffDTO>(await _unitOfWork.LeaveCompOff.GetByIdAsync(inputDTO.CompOffId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetComboff)}");
            throw;
        }
    }

    [HttpPost(Name = "GetComboffList")]
    public async Task<IActionResult> GetComboffList(Core.Helper.RequestParams requestParams, int? UnitId)
    {

        try
        {
            var returnData = (from compoff in _unitOfWork.LeaveCompOff.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId).Result
                              join emp in _unitOfWork.LeaveCalenderYear.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId).Result on compoff.CalendarYear equals emp.LeaveYearId
                              where (compoff.UnitId == UnitId && compoff.IsActive == true)

                              //var returnData = _mapper.Map<IList<LeaveCompOffDTO>>(await _unitOfWork.LeaveCompOff.GetAll(requestParams: requestParams,
                              //                                                                                 expression: (p => p.UnitId == UnitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                              //                                                                                 orderBy: (m => m.OrderBy(x => x.CalendarYear))));
                              //IList<LeaveCompOffDTO> outputModel = new List<LeaveCompOffDTO>();

                              //outputModel = returnData.Select(r => new LeaveCompOffDTO
                              //{
                              //    ApplicableDay = r.ApplicableDay,
                              //    ApplicableFor = r.ApplicableFor,
                              //    AvailDay = r.AvailDay,
                              //    CalendarYear = r.CalendarYear,
                              //    CompOffId = r.CompOffId,
                              //    MinFullDay = r.MinFullDay,
                              //    MinHalfDay = r.MinHalfDay,
                              //    LeavePolicy = r.LeavePolicy

                              //}).ToList();

                              select new LeaveCompOffDTO
                              {
                                  ApplicableDay = compoff.ApplicableDay,
                                  ApplicableFor = compoff.ApplicableFor,
                                  CompOffId = compoff.CompOffId,
                                  AvailDay = compoff.AvailDay,
                                  MinFullDay = compoff.MinFullDay,
                                  MinHalfDay = compoff.MinHalfDay,
                                  LeavePolicy = compoff.LeavePolicy,
                                  CalendarName = emp.CalendarName

                              }).ToList();

            return Ok(returnData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetComboffList)}");
            throw;
        }

    }


    [HttpPost(Name = "GetEmployeeComboffList")]
    public async Task<IActionResult> GetEmployeeComboffList(Core.Helper.RequestParams requestParams, int? UnitId, int? EmployeeId)
    {

        try
        {
            var returnData = _mapper.Map<IList<EmployeeCompOff>>(await _unitOfWork.EmployeeCompOff.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.UnitId == UnitId && p.EmployeeId == EmployeeId),
                                                                                             orderBy: (m => m.OrderBy(x => x.CompOffDate))));
            IList<EmployeeCompOffDTO> outputModel = new List<EmployeeCompOffDTO>();

            outputModel = returnData.Select(r => new EmployeeCompOffDTO
            {
                CreatedBy = r.CreatedBy,
                TicketId = r.TicketId,
                CompOffId = r.CompOffId,
                ApprovedBy = r.ApprovedBy,
                ApprovedOn = r.ApprovedOn,
                CompOffDate = r.CompOffDate,
                Status = r.Status,
                Remarks = r.Remarks,
                CompOffType = r.CompOffType,
                CreatedOn = r.CreatedOn

            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetComboffList)}");
            throw;
        }

    }


    [HttpPost(Name = "GetEmployeeComboffPendingList")]
    public async Task<IActionResult> GetEmployeeComboffPendingList(Core.Helper.RequestParams requestParams, int? UnitId)
    {

        try
        {
            var returnData = (from compoff in _unitOfWork.EmployeeCompOff.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId).Result
                              join emp in _unitOfWork.EmployeeMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId).Result on compoff.EmployeeId equals emp.EmployeeId
                              where (compoff.UnitId == UnitId && compoff.Status == 0)

                              select new EmployeeCompOffDTO
                              {
                                  CreatedBy = compoff.CreatedBy,
                                  TicketId = compoff.TicketId,
                                  CompOffId = compoff.CompOffId,
                                  ApprovedBy = compoff.ApprovedBy,
                                  ApprovedOn = compoff.ApprovedOn,
                                  CompOffDate = compoff.CompOffDate,
                                  Status = compoff.Status,
                                  Remarks = compoff.Remarks,
                                  CompOffType = compoff.CompOffType,
                                  CreatedOn = compoff.CreatedOn,
                                  EmployeeName = emp.EmployeeName,
                                  EmployeeCode = emp.EmployeeCode
                              }).ToList();

            //    );
            //IList<EmployeeCompOffDTO> outputModel = new List<EmployeeCompOffDTO>();

            //outputModel = returnData.Select(r => new EmployeeCompOffDTO
            //{
            //    CreatedBy = r.CreatedBy,
            //    TicketId = r.TicketId,
            //    CompOffId = r.CompOffId,
            //    ApprovedBy = r.ApprovedBy,
            //    ApprovedOn = r.ApprovedOn,
            //    CompOffDate = r.CompOffDate,
            //    Status = r.Status,
            //    Remarks = r.Remarks,
            //    CompOffType = r.CompOffType,
            //    CreatedOn = r.CreatedOn

            //}).ToList();
            return Ok(returnData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Calender Year {nameof(GetComboffList)}");
            throw;
        }

    }


    public async Task<IActionResult> GetEmployeeCompOffInfo(Core.Helper.RequestParams requestParams, int? employeeId, int? UnitId)
    {
        try
        {
            var returnData = (from compoff in _unitOfWork.EmployeeCompOff.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId).Result
                              join emp in _unitOfWork.EmployeeMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId).Result on compoff.EmployeeId equals emp.EmployeeId
                              where (compoff.UnitId == UnitId && compoff.Status == 0 && (emp.EmployeeId == employeeId || emp.ManagerId == employeeId))

                              select new EmployeeCompOffDTO
                              {
                                  CreatedBy = compoff.CreatedBy,
                                  TicketId = compoff.TicketId,
                                  CompOffId = compoff.CompOffId,
                                  ApprovedBy = compoff.ApprovedBy,
                                  ApprovedOn = compoff.ApprovedOn,
                                  CompOffDate = compoff.CompOffDate,
                                  Status = compoff.Status,
                                  Remarks = compoff.Remarks,
                                  CompOffType = compoff.CompOffType,
                                  CreatedOn = compoff.CreatedOn,
                                  EmployeeName = emp.EmployeeName,
                                  EmployeeCode = emp.EmployeeCode
                              }).ToList();

            return Ok(returnData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving pending employee compoff {nameof(GetEmployeeCompOffInfo)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCompOff(LeaveCompOffDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LeaveCompOff outputMaster = _mapper.Map<LeaveCompOff>(await _unitOfWork.LeaveCompOff.GetByIdAsync(inputDTO.CompOffId));
                outputMaster.IsActive = false;
                _unitOfWork.LeaveCompOff.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Comp Off {nameof(DeleteCompOff)}");
            throw;
        }
    }


    public async Task<string> LeaveCancelTicketRequest(int leaveId, int moduleId, string employeeId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@Id", leaveId, DbType.Int32);
            parms.Add(@"@ModuleId", moduleId, DbType.Int32);
            parms.Add(@"@CanceledBy", Convert.ToInt32(employeeId), DbType.Int32);

            try
            {
                await _unitOfWork.EmployeeLeaveDetails.GetStoredProcedure("usp_Canceltickets", parms);
            }
            catch (Exception ex) { return "Error while cancel/reject leave."; }
            try
            {
                //LeaveAction objLeaveAction = new LeaveAction();
                //objLeaveAction.TicketId = inputDTO.TicketId;
                //objLeaveAction.EmployeeId = inputDTO.EmployeeId;
                //objLeaveAction.Profile = path;
                //objLeaveAction.LeaveIds = Convert.ToString(inputDTO.LeaveTypeId);
                //  string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveRequalizeMail(userAction);
                string returnMessage = "Transaction Successful!";
                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in while cancel/reject leave {nameof(LeaveCancelTicketRequest)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<string> TicketRegularizeProcessing(LeaveAction userAction)
    {
        // string returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveRequalizeTestMail(userAction);
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@Ids", userAction.LeaveIds, DbType.String);
            parms.Add(@"@ActionType", userAction.ActionType, DbType.String);
            parms.Add(@"@TicektType", userAction.ActionRemarks, DbType.String);
            parms.Add(@"@ApprovedBy", userAction.ApprovedBy, DbType.String);
            try
            {
                await _unitOfWork.EmployeeLeaveDetails.GetStoredProcedure("usp_TicektRegularizeProcessing", parms);
            }
            catch (Exception ex) { return "Error while approve/reject."; }
            try
            {
                string returnMessage = "";
                //LeaveAction objLeaveAction = new LeaveAction();
                //objLeaveAction.TicketId = inputDTO.TicketId;
                //objLeaveAction.EmployeeId = inputDTO.EmployeeId;
                //objLeaveAction.Profile = path;
                //objLeaveAction.LeaveIds = Convert.ToString(inputDTO.LeaveTypeId);
                if (userAction.ActionRemarks == "L")
                    returnMessage = await _unitOfWork.EmployeeLeaveDetails.SendLeaveRequalizeMail(userAction);
                else if (userAction.ActionRemarks == "A")
                {
                    ManualPunchesAction attAction = new ManualPunchesAction();
                    attAction.ActionRemarks = "";
                    attAction.ManualPunchIds = userAction.LeaveIds;
                    attAction.ActionType = userAction.ActionType;
                    returnMessage = await _unitOfWork.ManualPunches.SendManualPunchActionMail(attAction);
                }

                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in while approve/reject tickets {nameof(TicketRegularizeProcessing)}");
            throw;
        }
    }

    [HttpPost(Name = "GetUnitHolidayList")]
    public async Task<int> GetUnitHolidayList(Core.Helper.RequestParams requestParams, DateTime? startDate, DateTime? endDate, int? unitId)
    {
        IList<UnitHolidayListDTO> outputModel = new List<UnitHolidayListDTO>();
        outputModel = _mapper.Map<IList<UnitHolidayListDTO>>(await _unitOfWork.UnitHolidayList.GetPagedListWithExpression(requestParams, p => (p.HolidayDate >= startDate && p.HolidayDate <= endDate) && p.UnitId == unitId && p.IsActive == true));
        return outputModel.Count;
    }

    [HttpPost(Name = "GetWeekendDay")]
    public string GetWeekendDay(int? unitId)
    {
        // UnitMasterDTO outputModel = new UnitMasterDTO();
        string outputModel = _mapper.Map<UnitMasterDTO>(_unitOfWork.UnitMaster.Find(x => x.UnitID == unitId)).WeeklyOff;
        //ObjectResult objResult = (ObjectResult)outputModel;
        // var objResultData = objResult.Value;

        return outputModel;
    }

    public async Task<EmployeeLeaveDetailsDTO> GetEmployeeDebitDetails(EmployeeLeaveBalanceInputs userAction)
    {
        EmployeeLeaveDetailsDTO EmployeeLeaveList = new EmployeeLeaveDetailsDTO();
        var parms = new DynamicParameters();
        parms.Add("@EmployeeId", userAction.EmployeeId, DbType.Int32);
        parms.Add("@LeaveTypeId", userAction.LeaveTypeId, DbType.Int32);
        parms.Add("@UnitId", userAction.UnitId, DbType.Int32);
        parms.Add("@FMonth", userAction.FromMonth, DbType.Int32);
        parms.Add("@TMonth", userAction.ToMonth, DbType.Int32);
        parms.Add("@FYear", userAction.FromYear, DbType.Int32);
        parms.Add("@TYear", userAction.ToYear, DbType.Int32);
        //string sQuery = $"select  a.*,details.LeaveType,details.LeaveTypeCode from EmployeeLeaveHistory a inner join [EmployeeLeaveDetails] l on a.ticketId=l.ticketId " +
        //    $"inner join [LeaveTypeMaster] details on a.LeaveTypeId=details.LeaveTypeId " +
        //    $"where   a.ticketId={ticketId} and a.LeaveStatus in (0,1) ORDER BY a.CreatedOn desc\r\n";
        EmployeeLeaveList.EmployeeLeaveList = (await _unitOfWork.EmployeeLeaveDetails.GetSPData<EmployeeLeaveDetailsDTO>("usp_GetDebitHistory", parms));
        return EmployeeLeaveList;
    }

    //public async Task<string> SendApprovalMail(LeaveAction userAction)
    //{
    //    bool isMailSend = false;
    //    IDbConnection IDBConn = DbConnection;
    //    if (IDBConn.State == ConnectionState.Closed)
    //    { IDBConn.Open(); }
    //    if (userAction != null)
    //    {

    //        int iCtr = 0;
    //        //createmail
    //        //string actionPath = "https://localhost:7151/";
    //        string actionPath = "https://simplihr2.azurewebsites.net/";
    //        string sSubject = "";
    //        StringBuilder mailBuilder = new StringBuilder();
    //        string sFileName = "LeaveApproval.html";
    //        string sTableData = string.Empty;
    //        try
    //        {
    //            String mailTemplate = MailHelper.GetMailTemplate(sFileName);
    //            List<LeaveAction> LeaveList = new List<LeaveAction>();
    //            LeaveList = await GetTableData<LeaveAction>(IDBConn, null, $" LeaveIds IN(SELECT value FROM STRING_SPLIT('{userAction.LeaveIds}',','))");
    //            if (LeaveList.Count > 0)
    //            {

    //                EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={LeaveList[0].EmployeeId}", "")).FirstOrDefault();
    //                EmployeeMaster managerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
    //                DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
    //                JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
    //                //List<ShiftMaster> shiftList = (await GetQueryAll<ShiftMaster>($"SELECT a.ShiftCode,a.ShiftName FROM ShiftMaster a INNER JOIN EmployeeMaster b ON a.ShiftIdAttended=b.ShiftCode WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId} AND a.UnitId = {employeeMasterInfo.UnitId}  AND a.IsActive=1", IDBConn, trans));
    //                mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
    //                mailTemplate = mailTemplate.Replace("#empImage#", "data:image/png;base64," + Convert.ToBase64String(employeeMasterInfo.ProfileImage, 0, employeeMasterInfo.ProfileImage.Length));
    //                mailTemplate = mailTemplate.Replace("#empDesig#", employeeDesig.JobTitle);
    //                mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
    //                mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
    //                mailTemplate = mailTemplate.Replace("#assignedOn#", managerData.EmployeeName);
    //                string dutyDates = string.Join(",", LeaveList.Select(x => $"'{x.ManualPunchDate.Value.ToString("yyyy-MM-dd")}'").Distinct());
    //                List<ManualPunches> punchesList = await GetTableData<ManualPunches>(IDBConn, null, $"EmployeeId={manualPunchList[0].EmployeeId} AND ManualPunchDate IN({dutyDates})", "");
    //                if (employeeMasterInfo != null)
    //                {
    //                    userAction.ManualPunchIds = "," + userAction.ManualPunchIds + ",";
    //                    foreach (var manualPunch in punchesList)
    //                    {
    //                        if (userAction.ManualPunchIds.Contains("," + manualPunch.ManualPunchId.ToString() + ","))
    //                        {
    //                            // var shiftCode = attendanceHistoryData.Where(x => x.DutyDate == manualPunch.ManualPunchDate).Select(r => r.ShiftIDAttended).FirstOrDefault().ToString();
    //                            // shiftCode = shiftList.Select(x => x.ShiftCode == shiftCode).FirstOrDefault().ToString();
    //                            iCtr = iCtr + 1;
    //                            var status = manualPunch.ActionType == "R" ? "Rejected" : manualPunch.ActionType == "A" ? "Approved" : "";
    //                            sTableData = sTableData + $"<tr style='background: #fff;'>" +
    //                                $"<td style='text-align: center;'>{iCtr.ToString()}</td>" +
    //                                $"<td style='padding: 10px;left'>{manualPunch.ManualPunchDate.Value.ToString("dd-MM-yyyy")}</td>" +
    //                                // $"<td style='text-align: center;'>{shiftCode}</td>" +
    //                                $"<td style='text-align: center;'>{manualPunch.ManualPunchInTime}</td>" +
    //                                $"<td style='text-align: center;'>{manualPunch.ManualPunchOutTime}</td>" +
    //                                 $"<td style='padding: 10px;'>{manualPunch.ManualPunchReason}</td>" +
    //                                 $"<td style='padding: 10px; center'>{status}</td>" +
    //                                $"<td style='padding: 10px;'>{manualPunch.ActionRemark}</td></tr>";
    //                            manualPunch.IsActionMailSent = true;
    //                        }
    //                    }
    //                    string manualPunchIDs = string.Join(",", punchesList.Select(x => $"{x.ManualPunchId}").Distinct());
    //                    mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);
    //                    string EmployeeEmailId = employeeMasterInfo.OfficialEmail;
    //                    //EmployeeEmailId= "juyalpradeep@gmail.com";
    //                    sSubject = $"SimpliHR2.0 leave approval request";

    //                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com");

    //                    if (isMailSend)
    //                        // await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
    //                        return "Success";
    //                }
    //                return "Employee details not found";
    //            }
    //            return "Leave not found";
    //        }
    //        catch (Exception ex)
    //        {
    //            return ex.InnerException.Message.ToString();
    //        }

    //    }
    //    return "Inputs are not correct";
    //    //return isMailSend;
    //}


}

