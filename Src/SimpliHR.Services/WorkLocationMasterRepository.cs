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

namespace SimpliHR.Services;

public class WorkLocationMasterRepository : GenericRepository<WorkLocationMaster>, IWorkLocationMasterRepository
{
	public WorkLocationMasterRepository(SimpliDbContext context): base(context)
	{
       
    }
}


