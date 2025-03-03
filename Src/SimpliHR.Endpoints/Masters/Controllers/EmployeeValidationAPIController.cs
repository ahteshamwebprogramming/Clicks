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
using SimpliHR.Infrastructure.Models.Exit;
using Microsoft.IdentityModel.Tokens;
using Dapper;
using System.Data;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeValidationAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeValidationAPIController> _logger;
    private readonly IMapper _mapper;

    public EmployeeValidationAPIController(IUnitOfWork unitOfWork, ILogger<EmployeeValidationAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<EmployeeValidationVM> SaveEmployeeValidation(EmployeeValidationVM empValidationVM)
    {
        string sQuery = string.Empty;
        bool isSuccess;      
        //CallSave
        sQuery = @"delete EmployeeValidation WHERE EmployeeValidationId=@EmployeeValidationId AND  UnitId=@UnitId";
        isSuccess = await _unitOfWork.EmployeeValidation.ExecuteListData<EmployeeValidation>(_mapper.Map<List<EmployeeValidation>>(empValidationVM.EmployeeValidationList.Where(x => x.EmployeeValidationId > 0).Select(p => p)), sQuery,true);
        sQuery = $@"insert EmployeeValidation(EmployeeValidationId,ClientId,UnitId,AddAttachment,EditAttachment,IsMandatory,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,IsActive)
                    values(@EmployeeValidationId,@ClientId,@UnitId,@AddAttachment,@EditAttachment,@IsMandatory,'{DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss")}',{empValidationVM.LogInUser},{empValidationVM.LogInUser},1)";
        isSuccess = await _unitOfWork.EmployeeValidation.ExecuteListData<EmployeeValidation>(_mapper.Map<List<EmployeeValidation>>(empValidationVM.EmployeeValidationList.Where(x=>x.EmployeeValidationId>0).Select(p=>p)), sQuery,true);
        _unitOfWork.Save();
        empValidationVM.DisplayMessage = isSuccess ? "Success" : "Fail";
        //empValidationVM.DisplayMessage = "SUCCESS";       
        return empValidationVM;
    }

    [HttpPost]
    public async Task<List<EmployeeValidationDTO>> GetEmployeeValidation(string screenName, string screenTab="", int? clientId=0,int? unitId=0)
    {
        List<EmployeeValidationDTO> empValidationList = new List<EmployeeValidationDTO>();
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@ClientId", clientId, DbType.Int32);
            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@ScreenName", screenName, DbType.String);
            parms.Add(@"@ScreenTab", screenTab, DbType.String);
            empValidationList = _mapper.Map<List<EmployeeValidationDTO>>(await _unitOfWork.EmployeeValidation.GetSPData("GetEmployeeValidation", parms));            
        }
        catch (Exception ex) {
            //employeeValidationVM.DisplayMessage = "Error fetchint Employee Validations";
        }
        return empValidationList;
    }

}

