using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Employee;
using System.Linq.Expressions;

namespace SimpliHR.Core.Repository;

public interface IEmployeeMasterRepository : IGenericRepository<EmployeeMaster>
{
    //public List<EmployeeMasterDataDTO> GetEmployeesInfo(params object[] parameters);
    public Task<IList<EmployeeMasterDTO>> GetEmployeesInfo(Expression<Func<EmployeeMaster, bool>> expression);
    public Task<IList<EmployeeMasterDTO>> GetLoginEmployeesInfo(Expression<Func<EmployeeMaster, bool>> expression);
    public Task<IList<EmployeeMasterDTO>> GetEmployeeListing(Expression<Func<EmployeeMaster, bool>> expression);
    //public Task<IList<EmployeeMasterDTO>> GetEmployeesInfo(int employeeId);
    //public Task<IList<EmployeeMasterDTO>> GetEmployeeInfoForLogin(Expression<Func<EmployeeMaster, bool>> expression);
}

