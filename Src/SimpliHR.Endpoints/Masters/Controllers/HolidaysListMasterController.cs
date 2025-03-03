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
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;
using Microsoft.AspNetCore.Components.Forms;
using SimpliHR.Infrastructure.Models.ClientManagement;
using Azure.Core;
using static Dapper.SqlMapper;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class HolidaysListMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HolidaysListMasterController> _logger;
    private readonly IMapper _mapper;

    public HolidaysListMasterController(IUnitOfWork unitOfWork, ILogger<HolidaysListMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetHolidaysList(HolidaysListMasterDTO inputDTO)
    {
        try
        {
            HolidaysListMasterDTO outputDTO = _mapper.Map<HolidaysListMasterDTO>(await _unitOfWork.HolidaysListMaster.GetByIdAsync(inputDTO.HolidayId));
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
            _logger.LogError(ex, $"Error in retriving Holidays List {nameof(GetHolidaysList)}");
            throw;
        }
    }

    [HttpPost(Name = "GetHolidaysLists")]
    public async Task<IActionResult> GetHolidaysLists(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<HolidaysListMasterDTO> outputModel = new List<HolidaysListMasterDTO>();
            outputModel = _mapper.Map<IList<HolidaysListMasterDTO>>(await _unitOfWork.HolidaysListMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Holidays Lists {nameof(GetHolidaysLists)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveHolidaysList(HolidaysListMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<HolidaysListMaster, bool>> expression = a => a.HolidayName.Trim().Replace(" ", "") == inputDTO.HolidayName.Trim().Replace(" ", "") && a.HolidayYear == inputDTO.HolidayYear && a.IsActive == true;
                if (!_unitOfWork.HolidaysListMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<HolidaysListMaster>(inputDTO);
                    _unitOfWork.HolidaysListMaster.AddAsync(_mapper.Map<HolidaysListMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Holidays List {nameof(SaveHolidaysList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateHolidaysList(HolidaysListMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<HolidaysListMaster, bool>> expression = a => a.HolidayName.Trim().Replace(" ", "") == inputDTO.HolidayName.Trim().Replace(" ", "") && a.HolidayYear == inputDTO.HolidayYear && a.HolidayId != inputDTO.HolidayId && a.IsActive == true;
                if (!_unitOfWork.HolidaysListMaster.Exists(expression))
                {
                    _unitOfWork.HolidaysListMaster.Update(_mapper.Map<HolidaysListMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in HolidaysList updates {nameof(UpdateHolidaysList)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteHolidaysList(HolidaysListMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                HolidaysListMaster outputMaster = _mapper.Map<HolidaysListMaster>(await _unitOfWork.HolidaysListMaster.GetByIdAsync(inputDTO.HolidayId));
                outputMaster.IsActive = false;
                _unitOfWork.HolidaysListMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting HolidaysList {nameof(DeleteHolidaysList)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<HolidaysListKeyValues> GetHolidaysListKeyValue()
    {
        return (_unitOfWork.HolidaysListMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new HolidaysListKeyValues()
                           {
                               HolidayId = p.HolidayId,
                               HolidayName = p.HolidayName
                           })).ToList();
    }

    #region "Unit Wise Holiday List"

    [HttpPost(Name = "GetUnitHolidaysList")]
    public async Task<IActionResult> GetUnitHolidaysList(Core.Helper.RequestParams requestParams, string sUnitId)
    {
        try
        {
            List<UnitHolidayListDTO> outputModel = new List<UnitHolidayListDTO>();
            outputModel = _mapper.Map<List<UnitHolidayListDTO>>(await _unitOfWork.UnitHolidayList.GetPagedListWithExpression(requestParams, p => sUnitId.Contains(","+p.UnitId.ToString()+",") && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Holidays Lists {nameof(GetHolidaysLists)}");
            throw;
        }
    }

    [HttpPost(Name = "GetUnitHolidayData")]
    public async Task<IActionResult> GetUnitHolidayData(UnitHolidayListDTO inputDTO)
    {
        try
        {
            UnitHolidayListDTO outputModel = new UnitHolidayListDTO();
            outputModel = _mapper.Map<UnitHolidayListDTO>(await _unitOfWork.UnitHolidayList.GetByIdAsync(inputDTO.UnitHolidayId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Holidays Lists {nameof(GetHolidaysLists)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveUnitHolidayList(UnitHolidayListDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<UnitHolidayList, bool>> expression = a => a.HolidayName.Trim().Replace(" ", "") == inputDTO.HolidayName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.HolidayYear == inputDTO.HolidayYear && a.IsActive == true;
                if (!_unitOfWork.UnitHolidayList.Exists(expression))
                {
                    if (inputDTO.HolidayId == null)
                        inputDTO.HolidayId = 0;
                    //var outputDTO = _mapper.Map<HolidaysListMaster>(inputDTO);
                    _unitOfWork.UnitHolidayList.AddAsync(_mapper.Map<UnitHolidayList>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Holidays List {nameof(SaveHolidaysList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateUnitHolidayList(UnitHolidayListDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<UnitHolidayList, bool>> expression = a => a.HolidayName.Trim().Replace(" ", "") == inputDTO.HolidayName.Trim().Replace(" ", "") && a.HolidayYear == inputDTO.HolidayYear && a.UnitHolidayId != inputDTO.UnitHolidayId && a.UnitId==inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.UnitHolidayList.Exists(expression))
                {
                    _unitOfWork.UnitHolidayList.Update(_mapper.Map<UnitHolidayList>(inputDTO));
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
            _logger.LogError(ex, $"Error in HolidaysList updates {nameof(UpdateHolidaysList)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetUnitHolidaysList(UnitHolidayListDTO inputDTO)
    {
        try
        {
            UnitHolidayListDTO outputDTO = _mapper.Map<UnitHolidayListDTO>(await _unitOfWork.UnitHolidayList.GetByIdAsync(inputDTO.UnitHolidayId));
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
            _logger.LogError(ex, $"Error in retriving Holidays List {nameof(GetHolidaysList)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUnitHolidaysList(UnitHolidayListDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                UnitHolidayList outputMaster = _mapper.Map<UnitHolidayList>(await _unitOfWork.UnitHolidayList.GetByIdAsync(inputDTO.UnitHolidayId));
                outputMaster.IsActive = false;
                _unitOfWork.UnitHolidayList.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting HolidaysList {nameof(DeleteHolidaysList)}");
            throw;
        }
    }

    #endregion "Unit Wise Holiday List"

    //Unit Mapping
    public async Task<List<UnitMasterDTO>> GetClientUnits(int clientId)
    {
        try
        {
            List<UnitMasterDTO> outputDTO = _mapper.Map<List<UnitMasterDTO>>(_unitOfWork.UnitMaster.Find(x => x.ClientId == clientId && x.IsActive == true));
            return outputDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting HolidaysList {nameof(GetClientUnits)}");
            throw;
        }
    }

    public async Task<UnitHolidayListVM> AssignUnitHolidays(UnitHolidayListVM holidayVm)
    {
        if(holidayVm.HolidayMasterList.Count<=0)
        {
            holidayVm.DisplayMessage = "No data found to save";
        }
        else
        {
            var selectedUnits = holidayVm.HolidayMasterList.Select(p=>p.UnitId).Distinct().ToList();
            foreach (var unit in selectedUnits)
            {
                List<UnitHolidayList> unitHolidays = new List<UnitHolidayList>();
                List<HolidaysListMasterDTO> holidaysListMasterDTO = new List<HolidaysListMasterDTO>();
                holidaysListMasterDTO = holidayVm.HolidayMasterList.Where(p => p.UnitId == unit).ToList();
                string holidayIds = "," + string.Join(",", holidaysListMasterDTO.Select(t => { return t.HolidayId; })) + ",";
                //string sSql = $"SELECT UnitHolidayId,HolidayId,HolidayType FROM UnitHolidayList WHERE UnitHolidayId IN({holidayIds})";
                unitHolidays =  _unitOfWork.UnitHolidayList.FindAllByExpression(r => r.UnitId== unit && holidayIds.Contains("," + r.HolidayId + ","));
                foreach (var holiday in unitHolidays)
                {
                    holiday.HolidayType = holidaysListMasterDTO.Where(x => x.HolidayId == holiday.HolidayId).Select(p => p.HolidayType).FirstOrDefault(); ;
                    _unitOfWork.UnitHolidayList.UpdateDbEntry(holiday, "HolidayType");
                    holidaysListMasterDTO.Remove(holidaysListMasterDTO.Where(x => x.HolidayId == holiday.HolidayId).FirstOrDefault());
                }
                unitHolidays.Clear();
                
                if (holidaysListMasterDTO.Count>0)
                {
                    List<UnitHolidayListDTO> unitHolidaysDTO = new List<UnitHolidayListDTO>();
                    List<HolidaysListMasterDTO> holidaysListDTO = new List<HolidaysListMasterDTO>();
                    holidaysListDTO = _mapper.Map<List<HolidaysListMasterDTO>>(_unitOfWork.HolidaysListMaster.FindAllByExpression(r => holidayIds.Contains("," + r.HolidayId + ",")));
                    unitHolidaysDTO = _mapper.Map<List<UnitHolidayListDTO>>(holidaysListDTO);
                    unitHolidaysDTO.ForEach(x => { x.UnitId = unit; x.HolidayType = holidaysListMasterDTO.Where(p => p.HolidayId == x.HolidayId).Select(r => r.HolidayType).FirstOrDefault(); });
                    _unitOfWork.UnitHolidayList.AddRangeAsync(_mapper.Map <List<UnitHolidayList>>(unitHolidaysDTO));
                    _unitOfWork.Save();
                }
            }
            
           
            holidayVm.DisplayMessage = "Success";

        }
        return holidayVm;
    }
}
