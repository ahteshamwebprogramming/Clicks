using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Data;
namespace SimpliHR.Endpoints.Payroll;

[Route("api/[controller]")]
[ApiController]
public class ReportsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportsAPIController> _logger;
    private readonly IMapper _mapper;

    public ReportsAPIController(IUnitOfWork unitOfWork, ILogger<ReportsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
      

    }

    [HttpPost]
    public async Task<List<RegimeSelectionReportDTO>> RegimeSelection(int? UnitId)
    {
        
        try
        {

            var parms = new DynamicParameters();           
            parms.Add(@"@UnitId", UnitId, DbType.Int32);
           
            try
            {
                var result = _mapper.Map<List<RegimeSelectionReportDTO>>(await _unitOfWork.RegimeSelectionReport.GetSPData("usp_GetRegimeReports", parms));
                // objResult = (SalarySummery)result;
                List<RegimeSelectionReportDTO>? objResultData = (List<RegimeSelectionReportDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(RegimeSelection)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<List<EmployeeLeaveBalanceReportDTO>> GetEmployeeLeaveBalanceReport(EmployeeLeaveBalanceInputs inputData)
    {

        try
        {

            var parms = new DynamicParameters();
            parms.Add("@UnitId", inputData.UnitId, DbType.Int32);
            parms.Add("@FMonth", inputData.FromMonth, DbType.Int32);
            parms.Add("@TMonth", inputData.ToMonth, DbType.Int32);
            parms.Add("@FYear", inputData.FromYear, DbType.Int32);
            parms.Add("@TYear", inputData.ToYear, DbType.Int32);
            parms.Add("@DeptId", inputData.DepartmentId, DbType.Int32);
            parms.Add("@LocId", inputData.LocationId, DbType.Int32);

            try
            {
                var result = _mapper.Map<List<EmployeeLeaveBalanceReportDTO>>(await _unitOfWork.EmployeeLeaveBalanceReport.GetSPData("usp_LeaveReport", parms));
                // objResult = (SalarySummery)result;
                List<EmployeeLeaveBalanceReportDTO>? objResultData = (List<EmployeeLeaveBalanceReportDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(RegimeSelection)}");
            throw;
        }
    }
}

