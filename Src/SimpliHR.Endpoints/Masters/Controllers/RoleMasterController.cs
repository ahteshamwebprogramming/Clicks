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
using Azure.Core;
using Microsoft.AspNetCore.Components.Forms;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class RoleMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleMasterController> _logger;
    private readonly IMapper _mapper;

    public RoleMasterController(IUnitOfWork unitOfWork, ILogger<RoleMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetRole(RoleMasterDTO inputDTO)
    {
        try
        {
            RoleMasterDTO outputDTO = _mapper.Map<RoleMasterDTO>(await _unitOfWork.RoleMaster.GetByIdAsync(inputDTO.RoleId));
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
            _logger.LogError(ex, $"Error in retriving Role {nameof(GetRole)}");
            throw;
        }
    }

    [HttpPost(Name = "GetRoles")]
    public async Task<IActionResult> GetRoles(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<RoleMasterDTO> outputModel = new List<RoleMasterDTO>();
            outputModel = _mapper.Map<IList<RoleMasterDTO>>(await _unitOfWork.RoleMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Roles {nameof(GetRoles)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveRole(RoleMasterDTO inputDTO)
    {
        try
        {


            if (ModelState.IsValid)
            {
                Expression<Func<RoleMaster, bool>> expression = a => a.RoleName.Trim().Replace(" ", "") == inputDTO.RoleName.Trim().Replace(" ", "") && a.IsActive == true && a.UnitId == inputDTO.UnitId;
                if (!_unitOfWork.RoleMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<RoleMaster>(inputDTO);
                    _unitOfWork.RoleMaster.AddAsync(_mapper.Map<RoleMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Role {nameof(SaveRole)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateRole(RoleMasterDTO inputDTO)
    {
        try
        {


            if (ModelState.IsValid)
            {
                Expression<Func<RoleMaster, bool>> expression = a => a.RoleName.Trim().Replace(" ", "") == inputDTO.RoleName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.RoleId != inputDTO.RoleId && a.IsActive == true && a.UnitId == inputDTO.UnitId;
                if (!_unitOfWork.RoleMaster.Exists(expression))
                {
                    _unitOfWork.RoleMaster.Update(_mapper.Map<RoleMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Role updates {nameof(UpdateRole)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole(RoleMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                RoleMaster outputMaster = _mapper.Map<RoleMaster>(await _unitOfWork.RoleMaster.GetByIdAsync(inputDTO.RoleId));
                outputMaster.IsActive = false;
                _unitOfWork.RoleMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Role {nameof(DeleteRole)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<RoleKeyValues> GetRoleKeyValue()
    {
        return (_unitOfWork.RoleMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new RoleKeyValues()
                           {
                               RoleId = p.RoleId,
                               RoleName = p.RoleName
                           })).ToList();
    }




}
