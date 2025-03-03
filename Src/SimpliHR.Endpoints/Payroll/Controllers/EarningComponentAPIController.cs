using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SimpliHR.Core.Entities;
using SimpliHR.Services.DBContext;
using SimpliHR.Infrastructure.Models.Payroll;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace SimpliHR.Endpoints.Payroll;

[Route("api/[controller]/[action]")]
[ApiController]
public class EarningComponentAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EarningComponentAPIController> _logger;
    private readonly IMapper _mapper;
   // private readonly SimpliDbContext _simpliDbContext;

    public EarningComponentAPIController(IUnitOfWork unitOfWork, ILogger<EarningComponentAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
       // _simpliDbContext = SimpliDbContext;
    }


    public async Task<PayrollComponentViewModel> GetEarningCompnent(int id)
    {
        PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();
        payrollEarningVM.PayrollEarningComponent = _mapper.Map<PayrollEarningComponentDTO>(await _unitOfWork.PayrollEarningComponent.FindByIdAsync(id));
        return payrollEarningVM;
    }

    public async Task<PayrollComponentViewModel> GetDeductionCompnent(int id)
    {
        PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();
        payrollEarningVM.PayrollDeductionComponent = _mapper.Map<PayrollDeductionComponentDTO>(await _unitOfWork.PayrollDeductionComponent.FindByIdAsync(id));
        return payrollEarningVM;
    }

    public async Task<PayrollComponentViewModel> GetReimbursementCompnent(int id)
    {
        PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();
        payrollEarningVM.PayrollReimbursementComponent = _mapper.Map<PayrollReimbursementComponentDTO>(await _unitOfWork.PayrollReimbursementComponent.FindByIdAsync(id));
        return payrollEarningVM;
    }

    public async Task<PayrollComponentViewModel> GetEarningComponentsList(PayrollComponentViewModel payrollEarningVM, int limit, int offset,int isFixed)
    {
        try
        {
            //int? isfixedval = 0;
            //if (isFixed == true)
            //    isfixedval = 1;
            //else
            //    isfixedval = 0;

            List<PayrollEarningComponentDTO> outputModel = new List<PayrollEarningComponentDTO>();

            string sQuery = "SELECT a.*,b.UnitName,c.SalaryComponentTitle FROM PayrollEarningComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.PayrollEarningComponent.UnitId +
            " INNER JOIN SalaryComponentMaster c ON c.SalaryComponentId = a.SalaryComponentId where c.isfixed= " + isFixed + "  ORDER BY SalaryComponentDisapyOrder";

            payrollEarningVM.PayrollEarningComponentList = _mapper.Map<List<PayrollEarningComponentDTO>>(await _unitOfWork.PayrollEarningComponent.GetTableData<PayrollEarningComponentDTO>(sQuery));

            payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();
            //}
            return payrollEarningVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }

    public async Task<PayrollComponentViewModel> GetAllEarningComponentsList(PayrollComponentViewModel payrollEarningVM, int limit, int offset, int isFixed)
    {
        try
        {
            //int? isfixedval = 0;
            //if (isFixed == true)
            //    isfixedval = 1;
            //else
            //    isfixedval = 0;

            List<PayrollEarningComponentDTO> outputModel = new List<PayrollEarningComponentDTO>();

            string sQuery = "SELECT a.*,b.UnitName,c.SalaryComponentTitle FROM PayrollEarningComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.PayrollEarningComponent.UnitId +
            " INNER JOIN SalaryComponentMaster c ON c.SalaryComponentId = a.SalaryComponentId ORDER BY SalaryComponentDisapyOrder";

            payrollEarningVM.PayrollEarningComponentList = _mapper.Map<List<PayrollEarningComponentDTO>>(await _unitOfWork.PayrollEarningComponent.GetTableData<PayrollEarningComponentDTO>(sQuery));

            payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();
            //}
            return payrollEarningVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }
    public async Task<PayrollComponentViewModel> GetDeductionComponentsList(PayrollComponentViewModel payrollEarningVM, int limit, int offset)
    {
        try
        {
            List<PayrollDeductionComponentDTO> outputModel = new List<PayrollDeductionComponentDTO>();

            string sQuery = "SELECT a.*,b.UnitName,c.SalaryComponentTitle FROM PayrollDeductionComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.PayrollDeductionComponent.UnitId +
            " INNER JOIN SalaryComponentMaster c ON c.SalaryComponentId = a.SalaryComponentId ORDER BY SalaryComponentDisapyOrder";

            payrollEarningVM.PayrollDeductionComponentList = _mapper.Map<List<PayrollDeductionComponentDTO>>(await _unitOfWork.PayrollDeductionComponent.GetTableData<PayrollDeductionComponentDTO>(sQuery));
            payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();
            return payrollEarningVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }


    public async Task<PayrollComponentViewModel> GetReimbursementComponentsList(PayrollComponentViewModel payrollEarningVM, int limit, int offset)
    {
        try
        {
            List<PayrollReimbursementComponentDTO> outputModel = new List<PayrollReimbursementComponentDTO>();

            string sQuery = "SELECT a.*,b.UnitName,c.SalaryComponentTitle FROM PayrollReimbursementComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.PayrollReimbursementComponent.UnitId +
            " INNER JOIN SalaryComponentMaster c ON c.SalaryComponentId = a.SalaryComponentId ORDER BY SalaryComponentDisapyOrder";

            payrollEarningVM.PayrollReimbursementComponentList = _mapper.Map<List<PayrollReimbursementComponentDTO>>(await _unitOfWork.PayrollReimbursementComponent.GetTableData<PayrollReimbursementComponentDTO>(sQuery));
            payrollEarningVM.PayrollReimbursementComponentList.Skip(offset * limit).Take(limit).ToList();
            return payrollEarningVM;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }

    public async Task<EmployeeSalaryTemplateMappingViewModel> SaveEmployeeSalaryTemplateMapping(EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM)
    {
        EmployeeSalaryTemplateMapping inputData = new EmployeeSalaryTemplateMapping();
        inputData = _mapper.Map<EmployeeSalaryTemplateMapping>(employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping);
        string sMsg = await _unitOfWork.EmployeeSalaryTemplateMapping.SaveEmployeeSalaryTemplateMapping(inputData, employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping.MappingEmployeeIds);
        employeeSalaryTemplateMappingVM.DisplayMessage = sMsg;      
        return employeeSalaryTemplateMappingVM;
    }

    public async Task<EmployeeSalaryTemplateMappingViewModel> GetEmployeeSalaryTemplateMappingList(EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM, int limit, int offset)
    {
        try
        {
            List<EmployeeSalaryTemplateMapping> outputModel = new List<EmployeeSalaryTemplateMapping>();

            string sQuery = $"SELECT a.EmployeeSalaryTemplateId,a.MappingName,a.SalaryTemplateId,a.EmployeesSelection,a.DepartmentId,c.TemplateName,a.IsActive " +
                            $"FROM EmployeeSalaryTemplateMapping a " +
                            $"INNER JOIN SalaryTemplates c ON c.SalaryTemplateId = a.SalaryTemplateId WHERE a.IsActive = 1 ";

            employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMappingList = _mapper.Map<List<EmployeeSalaryTemplateMappingDTO>>(await _unitOfWork.EmployeeSalaryTemplateMapping.GetTableData<EmployeeSalaryTemplateMappingDTO>(sQuery));
            employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMappingList.Skip(offset * limit).Take(limit).ToList();
            //employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateDetailList = _mapper.Map<List<EmployeeSalaryTemplateDetailDTO>>(await _unitOfWork.EmployeeSalaryTemplateDetail.GetQueryAll($"SELECT * FROM EmployeeSalaryTemplateDetail WHERE EmployeeSalaryTemplateId={employeeSalaryTemplateId}"));

            return employeeSalaryTemplateMappingVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }

    
    public async Task<EmployeeSalaryTemplateMappingViewModel> GetEmployeeSalaryTemplateMapping(int employeeSalaryTemplateId)
    {
        try
        {
            EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
            employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping = _mapper.Map<EmployeeSalaryTemplateMappingDTO> (await _unitOfWork.EmployeeSalaryTemplateMapping.FindByIdAsync(employeeSalaryTemplateId));
            employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateDetailList = _mapper.Map<List<EmployeeSalaryTemplateDetailDTO>>(await _unitOfWork.EmployeeSalaryTemplateDetail.GetQueryAll($"SELECT * FROM EmployeeSalaryTemplateDetail WHERE EmployeeSalaryTemplateId={employeeSalaryTemplateId}"));
            employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping.MappingEmployeeIds = string.Join(",", employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateDetailList.Select(x => x.EmployeeId));
            return employeeSalaryTemplateMappingVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }

    public async Task<EmployeeSalaryTemplateMappingViewModel> GetEmployeeSalaryTemplateDetail(int employeeSalaryTemplateId)
    {
        try
        {
            EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
            employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateDetailList = _mapper.Map<List<EmployeeSalaryTemplateDetailDTO>>(await _unitOfWork.EmployeeSalaryTemplateDetail.GetQueryAll($"SELECT * FROM EmployeeSalaryTemplateDetail WHERE EmployeeSalaryTemplateId={employeeSalaryTemplateId}"));
            employeeSalaryTemplateMappingVM.DisplayMessage = "Success";
            return employeeSalaryTemplateMappingVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEarningComponentsList)}");
            throw;
        }
    }

    public async Task<string> DeleteEmployeeSalaryTemplateMappingInfo(int employeeSalaryTemplateId)
    {
        string result = "";
        result = await _unitOfWork.EmployeeSalaryTemplateMapping.DeleteEmployeeSalaryTemplateMappingInfo(employeeSalaryTemplateId);
        return result;
    }

    public async Task<PayrollComponentViewModel> SaveEarningComponent(PayrollComponentViewModel payrollEarningVM)
    {
        if (payrollEarningVM.PayrollEarningComponent.SalaryComponentId > 0)
        {
            if (payrollEarningVM.PayrollEarningComponent.EarningId == 0)
            {
                var a = _unitOfWork.PayrollEarningComponent.GetFilter(x => x.SalaryComponentId == payrollEarningVM.PayrollEarningComponent.SalaryComponentId).Result;
                if (a == null)
                {
                    payrollEarningVM.PayrollEarningComponent.CreatedOn = DateTime.Now;
                    object value = await _unitOfWork.PayrollEarningComponent.AddAsync(_mapper.Map<PayrollEarningComponent>(payrollEarningVM.PayrollEarningComponent));
                    payrollEarningVM.DisplayMessage = "Success";
                }
                else
                    payrollEarningVM.DisplayMessage = "Duplicate component found";
            }
            else
            {
                payrollEarningVM.PayrollEarningComponent.ModifiedOn = DateTime.Now;
                object value = await _unitOfWork.PayrollEarningComponent.UpdateAsync(_mapper.Map<PayrollEarningComponent>(payrollEarningVM.PayrollEarningComponent));
                payrollEarningVM.DisplayMessage = "Success";

            }
        }
        else
            payrollEarningVM.DisplayMessage = "Please select an option";

        return payrollEarningVM;
    }
    public async Task<PayrollComponentViewModel> SaveDeductionComponent(PayrollComponentViewModel payrollEarningVM)
    {
        if (payrollEarningVM.PayrollDeductionComponent.SalaryComponentId > 0)
        {
            if (payrollEarningVM.PayrollDeductionComponent.DeductionId == 0)
            {
                var a = _unitOfWork.PayrollDeductionComponent.GetFilter(x => x.SalaryComponentId == payrollEarningVM.PayrollDeductionComponent.SalaryComponentId).Result;
                if (a == null)
                {
                    payrollEarningVM.PayrollDeductionComponent.CreatedOn = DateTime.Now;
                    object value = await _unitOfWork.PayrollDeductionComponent.AddAsync(_mapper.Map<PayrollDeductionComponent>(payrollEarningVM.PayrollDeductionComponent));
                    payrollEarningVM.DisplayMessage = "Success";
                }
                else
                    payrollEarningVM.DisplayMessage = "Duplicate component found";
            }
            else
            {
                payrollEarningVM.PayrollDeductionComponent.CreatedOn = DateTime.Now;
                object value = await _unitOfWork.PayrollDeductionComponent.UpdateAsync(_mapper.Map<PayrollDeductionComponent>(payrollEarningVM.PayrollDeductionComponent));
                payrollEarningVM.DisplayMessage = "Success";
            }
        }
        else
            payrollEarningVM.DisplayMessage = "Please select an option";

        return payrollEarningVM;
    }
    public async Task<PayrollComponentViewModel> SaveReimbursementComponent(PayrollComponentViewModel payrollEarningVM)
    {
        if (payrollEarningVM.PayrollReimbursementComponent.SalaryComponentId > 0)
        {
            if (payrollEarningVM.PayrollReimbursementComponent.ReimbursementId == 0)
            {
                var a = _unitOfWork.PayrollReimbursementComponent.GetFilter(x => x.SalaryComponentId == payrollEarningVM.PayrollReimbursementComponent.SalaryComponentId).Result;
                if (a == null)
                {
                    payrollEarningVM.PayrollReimbursementComponent.CreatedOn = DateTime.Now;
                    object value = await _unitOfWork.PayrollReimbursementComponent.AddAsync(_mapper.Map<PayrollReimbursementComponent>(payrollEarningVM.PayrollReimbursementComponent));
                    payrollEarningVM.DisplayMessage = "Success";
                }
                else
                    payrollEarningVM.DisplayMessage = "Duplicate component found";
            }
            else
            {
                payrollEarningVM.PayrollReimbursementComponent.CreatedOn = DateTime.Now;
                object value = await _unitOfWork.PayrollReimbursementComponent.UpdateAsync(_mapper.Map<PayrollReimbursementComponent>(payrollEarningVM.PayrollReimbursementComponent));
                payrollEarningVM.DisplayMessage = "Success";
            }
        }
        else
            payrollEarningVM.DisplayMessage = "Please select an option";

        return payrollEarningVM;
    }
    public async Task<SalaryTemplateDTOForSave> GetSalaryComponents(SalaryTemplateDTOForSave payrollEarningVM, int limit, int offset)
    {
        try
        {
            //List<PayrollEarningComponentDTO> outputModel = new List<PayrollEarningComponentDTO>();

            string sQuery = "SELECT a.*,b.UnitName FROM PayrollEarningComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.UnitId +
            " where a.IsActive=1";

            string sQuery2 = "SELECT a.*,b.UnitName FROM PayrollDeductionComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.UnitId +
          " where a.IsActive=1";

              string sQuery3 = "SELECT a.*,b.UnitName FROM PayrollReimbursementComponent a INNER JOIN UnitMaster b ON a.UnitId = b.UnitId AND b.UnitId = " + payrollEarningVM.UnitId +
            " where a.IsActive=1";
            payrollEarningVM.PayrollEarningComponentList = _mapper.Map<List<PayrollEarningComponentDTO>>(await _unitOfWork.PayrollEarningComponent.GetTableData<PayrollEarningComponentDTO>(sQuery));
            payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();          

            payrollEarningVM.PayrollDeductionComponentList = _mapper.Map<List<PayrollDeductionComponentDTO>>(await _unitOfWork.PayrollDeductionComponent.GetTableData<PayrollDeductionComponentDTO>(sQuery2));
            payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();

            payrollEarningVM.PayrollReimbursementComponentList = _mapper.Map<List<PayrollReimbursementComponentDTO>>(await _unitOfWork.PayrollReimbursementComponent.GetTableData<PayrollReimbursementComponentDTO>(sQuery3));
             payrollEarningVM.PayrollReimbursementComponentList.Skip(offset * limit).Take(limit).ToList();

            //}
            return payrollEarningVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetSalaryComponents)}");
            throw;
        }
    }

   



}

