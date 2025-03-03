using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using System.Net;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;
using Microsoft.Data.SqlClient;


namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class DepartmentMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepartmentMasterController> _logger;
    private readonly IMapper _mapper;

    public DepartmentMasterController(IUnitOfWork unitOfWork, ILogger<DepartmentMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetDepartment(DepartmentMasterDTO inputDTO)
    {
        try
        {
            DepartmentMasterDTO outputDTO = _mapper.Map<DepartmentMasterDTO>(await _unitOfWork.DepartmentMaster.GetByIdAsync(inputDTO.DepartmentId));
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
            _logger.LogError(ex, $"Error in retriving Bank {nameof(GetDepartment)}");
            throw;
        }
    }

    [HttpPost(Name = "GetDepartments")]
    public async Task<IActionResult> GetDepartments(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<DepartmentMasterDTO> outputModel = new List<DepartmentMasterDTO>();
            outputModel = _mapper.Map<IList<DepartmentMasterDTO>>(await _unitOfWork.DepartmentMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Banks {nameof(GetDepartments)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveDepartment(DepartmentMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<DepartmentMaster, bool>> expression = a => (a.DepartmentName.Trim().Replace(" ", "") == inputDTO.DepartmentName.Trim().Replace(" ", "") || a.DepartmentCode.Trim().Replace(" ", "") == inputDTO.DepartmentCode.Trim().Replace(" ", "")) && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.DepartmentMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<DepartmentMaster>(inputDTO);
                    _unitOfWork.DepartmentMaster.AddAsync(_mapper.Map<DepartmentMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving bank {nameof(SaveDepartment)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateDepartment(DepartmentMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<DepartmentMaster, bool>> expression = a => (a.DepartmentName.Trim().Replace(" ", "") == inputDTO.DepartmentName.Trim().Replace(" ", "") || a.DepartmentCode.Trim().Replace(" ", "") == inputDTO.DepartmentCode.Trim().Replace(" ", "")) && a.UnitId == inputDTO.UnitId && a.DepartmentId != inputDTO.DepartmentId && a.IsActive == true;
                if (!_unitOfWork.DepartmentMaster.Exists(expression))
                {
                    _unitOfWork.DepartmentMaster.Update(_mapper.Map<DepartmentMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in bank updates {nameof(UpdateDepartment)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDepartment(DepartmentMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                DepartmentMaster outputMaster = _mapper.Map<DepartmentMaster>(await _unitOfWork.DepartmentMaster.GetByIdAsync(inputDTO.DepartmentId));
                outputMaster.IsActive = false;
                _unitOfWork.DepartmentMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting bank {nameof(DeleteDepartment)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<DepartmentKeyValues> GetDepartmentKeyValue()
    {
        return (_unitOfWork.DepartmentMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new DepartmentKeyValues()
                           {
                               DepartmentId = p.DepartmentId,
                               DepartmentName = p.DepartmentName
                           })).ToList();
    }

    [HttpPost]
    public IEnumerable<DepartmentMasterDTO> GetDepartmentsByUnitId(int UnitId)
    {
        string query = "Select * from DepartmentMaster where UnitId=@UnitId and IsActive=1";
        //var parameters = new { @UnitId = UnitId };
        var unitIdParameter = new SqlParameter("@UnitId", UnitId);
        IEnumerable<DepartmentMasterDTO> departments = _mapper.Map<List<DepartmentMasterDTO>>(_unitOfWork.DepartmentMaster.GetWithRawSql(query, unitIdParameter));
        return departments;
    }





}
