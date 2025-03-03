using SimpliHR.Services.DBContext;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.Employee;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Helper;
using Microsoft.IdentityModel.Tokens;

namespace SimpliHR.Services;

public class EmployeeMasterRepository : GenericRepository<EmployeeMaster>, IEmployeeMasterRepository
{
    private readonly SimpliDbContext _context;
    private readonly IMapper _mapper;
    //private readonly IUnitOfWork _unitOfWork;
    public EmployeeMasterRepository(SimpliDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;       
    }

    public async Task<IList<EmployeeMasterDTO>> GetEmployeeListing(Expression<Func<EmployeeMaster, bool>> expression)
    {
        IList<EmployeeMasterDTO> outputData = new List<EmployeeMasterDTO>();
        outputData = _mapper.Map<IList<EmployeeMasterDTO>>(_context.EmployeeMasters.Where(expression)
        //.Include(x => x.Department).Include(x => x.WorkLocation)
        // .Include(x => x.EmployeeBankDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeContactDetails.Where(p => p.IsActive == true))
        //.Include(x => x.EmployeeFamilyDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeAcademicDetails.Where(p => p.IsActive == true))
        //.Include(x => x.EmployeeExperienceDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeCertificationDetails.Where(p => p.IsActive == true))
        //.Include(x => x.EmployeeReferenceDetails.Where(p => p.IsActive == true))
        .Select(p => p));
        //.Include(x => x.EmployeeLanguageDetails.Where(p => p.IsActive == true))

        foreach (var employee in outputData)
        {
            employee.ManagerName = "";
            employee.HODName = "";
            employee.EnycEmployeeId = CommonHelper.Encrypt(Convert.ToString(employee.EmployeeId));
            if (!(employee.ManagerId == null || employee.ManagerId == 0))
            {
                employee.ManagerName = _context.EmployeeMasters.Where(x => x.EmployeeId == employee.ManagerId).Select(p => p.EmployeeName.IsNullOrEmpty() ? "" : p.EmployeeName).FirstOrDefault().ToString();
            }
            if (!(employee.HODId == null || employee.HODId == 0))
            {
                employee.HODName = _context.EmployeeMasters.Where(x => x.EmployeeId == employee.HODId).Select(p => p.EmployeeName.IsNullOrEmpty() ? "" : p.EmployeeName).FirstOrDefault().ToString();
            }
            if (!(employee.JobTitleId == null || employee.JobTitleId == 0))
            {
                var jobtitlemaster = _context.JobTitleMasters.Where(x => x.JobTitleId == employee.JobTitleId);
                if (jobtitlemaster.Count() > 0)
                {
                    employee.JobTitleName = jobtitlemaster.Select(p => p.JobTitle).FirstOrDefault().ToString();
                }
                else
                {
                    employee.JobTitleName = "";
                }
            }
            if (!(employee.DepartmentId == null || employee.DepartmentId == 0))
            {
                var departmentMasters = _context.DepartmentMasters.Where(x => x.DepartmentId == employee.DepartmentId);
                if (departmentMasters.Count() > 0)
                {
                    employee.DepartmentName = departmentMasters.Select(p => p.DepartmentName).FirstOrDefault().ToString();
                }
                else
                {
                    employee.DepartmentName = "";
                }
            }

        }
        return outputData;

    }

    public async Task<IList<EmployeeMasterDTO>> GetLoginEmployeesInfo(Expression<Func<EmployeeMaster, bool>> expression)
    {
        IList<EmployeeMasterDTO> outputData = new List<EmployeeMasterDTO>();

        outputData = _mapper.Map<IList<EmployeeMasterDTO>>(_context.EmployeeMasters.Where(expression)
        .Select(p => p));
        return outputData;

    }

   


    public async Task<IList<EmployeeMasterDTO>> GetEmployeesInfo(Expression<Func<EmployeeMaster, bool>> expression)
    {
        try
        {
            IList<EmployeeMasterDTO> outputData = new List<EmployeeMasterDTO>();
            outputData = _mapper.Map<IList<EmployeeMasterDTO>>(_context.EmployeeMasters.Where(expression).Include(x => x.Department).Include(x => x.WorkLocation)
                .Include(x => x.EmployeeBankDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeContactDetails.Where(p => p.IsActive == true))
            .Include(x => x.EmployeeFamilyDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeAcademicDetails.Where(p => p.IsActive == true))
            .Include(x => x.EmployeeExperienceDetails.Where(p => p.IsActive == true)).Include(x => x.EmployeeCertificationDetails.Where(p => p.IsActive == true))
            .Include(x => x.EmployeeReferenceDetails.Where(p => p.IsActive == true)).Select(p => p)
            .Include(x => x.EmployeeLanguageDetails.Where(p => p.IsActive == true)).Select(p => p));
            //.Include(x => x.EmployeeLanguageDetails.Where(p => p.IsActive == true))

            foreach (var employee in outputData)
            {
                employee.ManagerName = "";
                employee.HODName = "";
                employee.EnycEmployeeId = CommonHelper.Encrypt(Convert.ToString(employee.EmployeeId));
                if (!(employee.ManagerId == null || employee.ManagerId == 0))
                {
                    employee.ManagerName = _context.EmployeeMasters.Where(x => x.EmployeeId == employee.ManagerId).Select(p => p.EmployeeName.IsNullOrEmpty() ? "" : p.EmployeeName).FirstOrDefault().ToString();
                }
                if (!(employee.HODId == null || employee.HODId == 0))
                {
                    employee.HODName = _context.EmployeeMasters.Where(x => x.EmployeeId == employee.HODId).Select(p => p.EmployeeName.IsNullOrEmpty() ? "" : p.EmployeeName).FirstOrDefault().ToString();
                }
                if (!(employee.JobTitleId == null || employee.JobTitleId == 0))
                {
                    var jobtitlemaster = _context.JobTitleMasters.Where(x => x.JobTitleId == employee.JobTitleId);
                    if (jobtitlemaster.Count() > 0)
                    {
                        employee.JobTitleName = jobtitlemaster.Select(p => p.JobTitle).FirstOrDefault().ToString();
                    }
                    else
                    {
                        employee.JobTitleName = "";
                    }
                }


            }
            return outputData;
        }

        catch (Exception ex)
        {
            return null;
        }
    }
    //public async Task<IList<EmployeeMasterDTO>> GetEmployeesInfo(int employeeId)
    //{

    //}
}


