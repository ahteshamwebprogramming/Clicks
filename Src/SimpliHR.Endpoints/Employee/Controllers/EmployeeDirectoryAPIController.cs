using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Masters;
using System.Data;
namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeDirectoryAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeDirectoryAPIController> _logger;
    private readonly IMapper _mapper;

    public EmployeeDirectoryAPIController(IUnitOfWork unitOfWork, ILogger<EmployeeDirectoryAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<MainDirectoryDTO> GetEmployeeDirectoryDetails(int? unitId)
    {
        MainDirectoryDTO DirectoryList = new MainDirectoryDTO();
        var parms = new DynamicParameters();
        parms.Add(@"UnitId", unitId, DbType.Int32);       

        try
        {

            DirectoryList.MainDirectoryList = _mapper.Map<List<MainDirectoryDTO>>(await _unitOfWork.MainDirectory.GetSPData("usp_GetEmployeeDirecotryList", parms));
            return DirectoryList;
        }
        catch (SystemException ex)
        {
            return null;
        }

    }

    [HttpPost]
    public async Task<string> SaveEmployeeDirectory(EmployeeDirectoryAction userAction)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeDirectoryIds", userAction.EmployeeDirectoryIds, DbType.String);           
            parms.Add(@"@IsActives", userAction.IsActives, DbType.String);
            parms.Add(@"@UnitId", userAction.UnitId, DbType.Int32);
            parms.Add(@"@PostionIds", userAction.PositionIds, DbType.String);

            try
            {
                await _unitOfWork.QuickAccessUnitList.GetStoredProcedure("usp_SaveEmployeeDirectory", parms);
            }
            catch (Exception ex) { return "Error while saving links."; }
            try
            {
                string returnMessage = "SUCCESS"; // await _unitOfWork.ManualPunches.SendApprovalMail(userAction);

                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in while saving Employee Directory {nameof(SaveEmployeeDirectory)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeDirectory(int? UnitId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", UnitId, DbType.Int32);
         //   parms.Add(@"@EmpName", EmpName, DbType.String);

            IList<members> outputModel = new List<members>();          
            outputModel = _mapper.Map<IList<members>>(await _unitOfWork.EmployeeDashboardDetails.GetSPData<members>("usp_GetAllEmployeeDirectoryDetails", parms));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeDirectory)}");
            throw;

        }
    }

    public async Task<int> PositionIsExit(int? unitId,int? positionId)
    {
       // MainDirectoryDTO DirectoryList = new MainDirectoryDTO();
        var parms = new DynamicParameters();
        parms.Add(@"UnitId", unitId, DbType.Int32);
        parms.Add(@"PositionId", positionId, DbType.Int32);

        try
        {

            var result = _mapper.Map<List<EmployeeDirectoryDTO>>(await _unitOfWork.EmployeeDirectory.GetSPData("usp_PositionIsExist", parms));
            if (result.Count > 0)
                return  Convert.ToInt32(result[0].PositionId);
            else
                return 0;
        }
        catch (SystemException ex)
        {
            return 0;
        }

    }


    [HttpPost]
    public async Task<IActionResult> GetEmployeeDirectoryDetails(int UnitId,int employeeId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", UnitId, DbType.Int32);
            parms.Add(@"@EmployeeId", employeeId, DbType.Int32);

            IList<EmployeeDirectoryCardDetailsDTO> outputModel = new List<EmployeeDirectoryCardDetailsDTO>();
            outputModel = _mapper.Map<IList<EmployeeDirectoryCardDetailsDTO>>(await _unitOfWork.EmployeeDirectoryCardDetails.GetSPData<EmployeeDirectoryCardDetailsDTO>("usp_GetEmployeeIdCardDetails", parms));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeDirectoryDetails)}");
            throw;

        }
    }
}

