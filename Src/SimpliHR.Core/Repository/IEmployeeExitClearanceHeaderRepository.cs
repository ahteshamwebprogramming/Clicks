
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;


namespace SimpliHR.Core.Repository;

public interface IEmployeeExitClearanceHeaderRepository : IDapperRepository<EmployeeExitClearanceHeader>
{
    Task<string> SaveExitClearanceInfo(List<EmployeeExitClearanceHeader> HeaderList, List<EmployeeExitClearanceDetail> DetailList);
}

