using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;

namespace SimpliHR.Endpoints.TicketMaster;

[Route("api/[controller]")]
[ApiController]
public class TicketMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TicketMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public TicketMasterAPIController(IUnitOfWork unitOfWork, ILogger<TicketMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> CreateTicket(TicketMasterDTO inputDTO, string moduleCode)
    {
        try
        {
            if (inputDTO != null)
            {

                int res = (await _unitOfWork.TicketMaster.GetTableData<int>("insert into [TicketMaster] (ModuleId,TicketSource,CreatedBy,CreatedOn,Status,IsActive,UnitId) Values (" + inputDTO.ModuleId + ",'" + inputDTO.TicketSource + "','" + inputDTO.CreatedBy + "','" + ((DateTime)inputDTO.CreatedOn).ToString("MM-dd-yyyy") + "','" + inputDTO.Status + "',1," + inputDTO.UnitId + ") select @@identity")).FirstOrDefault();

                inputDTO.TicketId = res;
                inputDTO.TicketCode = moduleCode + res;

                await _unitOfWork.TicketMaster.RunSQLCommand("update TicketMaster set TicketCode='" + inputDTO.TicketCode + "' where TicketId=" + inputDTO.TicketId + "");

                //TicketMasterDTO ticketMasterDTO = new TicketMasterDTO();
                //ticketMasterDTO.TicketId = inputDTO.TicketId;
                //ticketMasterDTO.TicketCode = moduleCode + res;
                //await _unitOfWork.TicketMaster.UpdateAsync(_mapper.Map<Core.Entities.TicketMaster>(ticketMasterDTO));
                //_unitOfWork.Save();
                return Ok(inputDTO);

            }
            return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CreateTicket)}");
            throw;
        }
    }

    public async Task<IActionResult> GetTickets(int UnitId)
    {
        try
        {

            List<TicketMasterDTO> outputModel = new List<TicketMasterDTO>();

            //string sQuery = "select ecm.ClearanceMappingId,ecm.PrimaryClearancePerson,ecm.SecondaryClearancePerson,ecm.DepartmentId,em.employeename as PrimaryClearancePersonName,em1.employeename as SecondaryClearancePersonName,dm.DepartmentName as DepartmentName from  [dbo].[ExitClearanceMapping] ecm  join employeemaster em on ecm.PrimaryClearancePerson=em.employeeid join employeemaster em1 on ecm.SecondaryClearancePerson=em1.employeeid join departmentmaster dm on ecm.DepartmentId=dm.departmentId where ecm.isactive=1 and ecm.unitid=" + unitId + "";
            string sQuery = "select tm.*,(select EmployeeName from employeemaster em where em.EmployeeCode=tm.CreatedBy) CreatedByName from [dbo].[TicketMaster] tm where tm.unitid=" + UnitId + " and tm.isactive=1";

            List<TicketMasterDTO> dto = await _unitOfWork.TicketMaster.GetTableData<TicketMasterDTO>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTickets)}");
            throw;
        }
    }

   
}

