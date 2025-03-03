using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using SimpliHR.Services.DBContext;
using System.Text.RegularExpressions;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Common;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeFamilyDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeFamilyDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeFamilyDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeFamilyDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeFamilyDetail(EmployeeFamilyDetailDTO inputDTO)
    {
        try
        {
            EmployeeFamilyDetailDTO outputDTO = _mapper.Map<EmployeeFamilyDetailDTO>(await _unitOfWork.EmployeeFamilyDetail.GetByIdAsync(inputDTO.EmployeeFamilyDetailId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            //  outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee Family Details {nameof(GetEmployeeFamilyDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeFamilyList(Core.Helper.RequestParams requestParams, int? employeeID)
    {
        try
        {
            IList<EmployeeFamilyDetailDTO> outputModel = new List<EmployeeFamilyDetailDTO>();
            outputModel = _mapper.Map<IList<EmployeeFamilyDetailDTO>>(await _unitOfWork.EmployeeFamilyDetail.GetAll(requestParams, p => p.IsActive == true && p.EmployeeId == employeeID, null)).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees Family Details {nameof(GetEmployeeFamilyList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeFamilyDetail(EmployeeFamilyDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                string sDuplicateNotAllowedFor = ",Father,Mother,Husband,";
                Expression<Func<EmployeeFamilyDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && sDuplicateNotAllowedFor.Contains(","+a.Relationship.Trim()+",") && a.Relationship.Trim().Replace(" ", "") == inputDTO.Relationship && a.IsActive == true;
                if (!_unitOfWork.EmployeeFamilyDetail.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeFamilyDetail>(inputDTO);
                    int detailId = _unitOfWork.EmployeeFamilyDetail.Insert(_mapper.Map<EmployeeFamilyDetail>(inputDTO)).EmployeeFamilyDetailId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeFamilyDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeFamilyDetailId);
                    return Ok(detailId);
                }
                else
                    return Ok("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Academic Detail {nameof(SaveEmployeeFamilyDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEditEmployeeFamilyDetail(EmployeeFamilyDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                string sDuplicateNotAllowedFor = ",Father,Mother,Husband,";
                Expression<Func<EmployeeFamilyDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && sDuplicateNotAllowedFor.Contains("," + a.Relationship.Trim() + ",") && a.Relationship.Trim().Replace(" ", "") == inputDTO.Relationship && a.IsActive == true;
                
                //Expression<Func<EmployeeFamilyDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && (a.Relationship == null ? "" : a.Relationship).Trim().Replace(" ", "") == (inputDTO.Relationship == null ? "" : inputDTO.Relationship).Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.EmployeeFamilyDetail.Exists(expression))
                {
                    inputDTO.IsActive = false;
                    int detailId = _unitOfWork.EmployeeFamilyDetail.Insert(_mapper.Map<EmployeeFamilyDetail>(inputDTO)).EmployeeFamilyDetailId;
                    _unitOfWork.Save();
                    return Ok(detailId);
                }
                else
                    return BadRequest("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Academic Detail {nameof(SaveEmployeeFamilyDetail)}");
            throw;
        }
    }

    //public async Task<IActionResult> SaveDeleteEmployeeDetailForApproval(AddDeleteTableActionDTO? deleteEmployeeData)
    //{
    //    try
    //    {
    //        string sSql = "INSERT INTO [dbo].[AddDeleteTableAction]" +
    //            "([ActionBy],[ActionStatus],[ActionType],[TicketId],[Ref" +
    //            "erenceTable],[ReferenceId],[EmployeeId],[EntrySource],[CreatedOn]" +
    //            ",[CreatedBy],[IsActive])" +
    //            "VALUES(" + deleteEmployeeData.ActionBy + "," + deleteEmployeeData.ActionStatus + ",'" + deleteEmployeeData.ActionType
    //            + "'," + deleteEmployeeData.TicketId + ",'" + deleteEmployeeData.ReferenceTable + "'," + deleteEmployeeData.ReferenceId + "," + deleteEmployeeData.EmployeeId
    //            + ",'" + deleteEmployeeData.EntrySource + "','" + DateTime.Now + "'," + deleteEmployeeData.LoggedInUser + "," + deleteEmployeeData.IsActive + ")";
    //        await _unitOfWork.EditEmployeeData.RunSQLCommand(sSql);

    //        return Ok("Success");

    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveDeleteEmployeeDetailForApproval)}");
    //        throw;
    //    }

    //}

    [HttpPost]
    public IActionResult UpdateEmployeeFamilyDetail(EmployeeFamilyDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeFamilyDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && (a.Relationship == null ? "" : a.Relationship).Trim().Replace(" ", "") == (inputDTO.Relationship == null ? "" : inputDTO.Relationship).Trim().Replace(" ", "") && a.EmployeeFamilyDetailId != inputDTO.EmployeeFamilyDetailId && a.IsActive == true;
                if (!_unitOfWork.EmployeeFamilyDetail.Exists(expression))
                {
                    _unitOfWork.EmployeeFamilyDetail.Update(_mapper.Map<EmployeeFamilyDetail>(inputDTO));
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
            _logger.LogError(ex, $"Error in Employee Academic Detail updates {nameof(UpdateEmployeeFamilyDetail)}");
            throw;
        }
    }

    public async Task<IActionResult> DeleteEmployeeFamilyDetail(EmployeeFamilyDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeFamilyDetail dto = await _unitOfWork.EmployeeFamilyDetail.GetByIdAsync(inputDTO.EmployeeFamilyDetailId);
                dto.IsActive = false;
                _unitOfWork.EmployeeFamilyDetail.Update(_mapper.Map<EmployeeFamilyDetail>(dto));
                _unitOfWork.Save();
                return Ok("Success");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Experience Detail updates {nameof(DeleteEmployeeFamilyDetail)}");
            throw;
        }
    }





}
