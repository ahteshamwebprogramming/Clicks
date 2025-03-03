using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Models.Employee;
using System.Data;

[Route("api/[controller]/[action]")]
[ApiController]
  public class EmployeeTicketController : ControllerBase
   {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeTicketController> _logger;
    private readonly IMapper _mapper;

    public EmployeeTicketController(IUnitOfWork unitOfWork, ILogger<EmployeeTicketController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeTickets(EmployeeTicketsInputs inputs)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@ModuleId", inputs.ModuleId, DbType.Int32);
            parms.Add(@"@Status", inputs.Status, DbType.Int32);
            parms.Add(@"@TicketId", inputs.TicketId, DbType.String);
            parms.Add(@"@StartDate", inputs.StartDate, DbType.String);
            parms.Add(@"@EndDate", inputs.EndDate, DbType.String);
            parms.Add(@"@MgrId", inputs.MgrId, DbType.Int32);
            parms.Add(@"@UnitId", inputs.UnitId, DbType.Int32);
            parms.Add(@"@EmployeeId", inputs.EmployeeId, DbType.Int32);
            parms.Add(@"@IsAdmin", inputs.IsAdmin, DbType.Int32);
            //IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            IList<EmployeeTicketsViewDTO> outputModel = new List<EmployeeTicketsViewDTO>();
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetEmployeesInfo(p =>p.UnitId== inputDTO.UnitId && p.Dob.Value.Day == DateTime.Now.Day && p.Dob.Value.Month == DateTime.Now.Month)).ToList();
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.FindAllByExpression(p => p.UnitId == inputDTO.UnitId && p.Dob.Value.Day == DateTime.Now.Day && p.Dob.Value.Month == DateTime.Now.Month));
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.CallStoredProc("usp_GetEmployeeBirthDay").ToList());
            outputModel = _mapper.Map<IList<EmployeeTicketsViewDTO>>(await _unitOfWork.EmployeeTicketsView.GetSPData<EmployeeTicketsViewDTO>("usp_GetTicketsDetails", parms));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeTickets)}");
            throw;

        }
    }

}

