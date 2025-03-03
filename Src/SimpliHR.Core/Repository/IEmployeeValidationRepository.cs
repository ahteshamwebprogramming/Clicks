
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Core.Repository;

public interface IEmployeeValidationRepository : IDapperRepository<EmployeeValidation>
{
   // Task<List<EmployeeValidation>> GetClientValidation(string? screenName,string? screenTab, int? clientId=0);
}

