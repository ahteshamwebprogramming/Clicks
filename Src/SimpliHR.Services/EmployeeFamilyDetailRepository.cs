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

namespace SimpliHR.Services;

public class EmployeeFamilyDetailRepository : GenericRepository<EmployeeFamilyDetail>, IEmployeeFamilyDetailRepository
{
	public EmployeeFamilyDetailRepository(SimpliDbContext context): base(context)
	{
       
    }
}


