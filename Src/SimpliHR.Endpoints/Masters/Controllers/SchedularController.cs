
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Models.Masters;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class SchedularController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SchedularController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public SchedularController(IUnitOfWork unitOfWork, ILogger<SchedularController> logger, IMapper mapper, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;
        //_connectionString = _configuration.GetConnectionString("SimplyConnectionDB");
    }
   
    public  IActionResult SchedularEvent(string? unitIds,string employeeId)
    {
        try
        {
          

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                using (SqlCommand cmd = new SqlCommand("UpdateLeaveBalance", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitIds", SqlDbType.NVarChar).Value = unitIds;
                    cmd.Parameters.Add("@EmployeeIds", SqlDbType.NVarChar).Value = employeeId;
                    cmd.Parameters.Add("@LeaveTypeIds", SqlDbType.NVarChar).Value = "";
                    cmd.ExecuteNonQuery();



                }

            }

            return Ok("Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Banks {nameof(SchedularEvent)}");
            throw;
        }
    }

    public IActionResult AddEmployeeLeaveBalance(string? unitIds, string employeeId)
    {
        try
        {


            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                using (SqlCommand cmd = new SqlCommand("AddEmployeeLeaveBalance", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitIds", SqlDbType.NVarChar).Value = unitIds;
                    cmd.Parameters.Add("@EmployeeIds", SqlDbType.NVarChar).Value = employeeId;
                    cmd.Parameters.Add("@LeaveTypeIds", SqlDbType.NVarChar).Value = "";
                    cmd.ExecuteNonQuery();



                }

            }

            return Ok("Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Banks {nameof(SchedularEvent)}");
            throw;
        }
    }
}

