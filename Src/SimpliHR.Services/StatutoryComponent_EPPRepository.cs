using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;
using System.Data;
using System.Text;
using SimpliHR.Infrastructure.Models.StatutoryComponent;
using System.Data.Entity;
using System.Drawing.Text;

namespace SimpliHR.Services;

public class StatutoryComponent_EPFRepository : GenericRepository<StatutoryComponent_EPF>, IStatutoryComponent_EPFRepository
{
    public StatutoryComponent_EPFRepository(SimpliDbContext context) : base(context)
    {

    }

    public async Task<string> SaveEmployeeEPFMapping(StatutoryComponent_EPF epfEmployeeMapping, string mappedIDs)
    {
        string sMsg = string.Empty;
        StringBuilder sData = new StringBuilder();
        string unMappedIds = string.Empty;
        string sQuery = string.Empty;
        string MappingEmployeeIds = mappedIDs;
        //using var dbContext = new SimpliDbContext();
        //using var trans = dbContext.Database.BeginTransaction();

        if (mappedIDs == null)
        {
            sMsg = "Employee/s not selected";
            return sMsg;
        }

        try
        {
            int mappingId = 0;
            int statutoryComponentsId = 0;
            string[] arrMappedId = mappedIDs.Split(",");
            EpfemployeeMapping[] unMappedEmployeeMapDetailParam = new EpfemployeeMapping[arrMappedId.Length];

            if (epfEmployeeMapping.StatutoryComponentsId == 0)
            {
                epfEmployeeMapping.CreatedDate = DateTime.Now;
                epfEmployeeMapping.IsActive = true;
                //Save Staturory
                epfEmployeeMapping = AddAsyncGetEntity(epfEmployeeMapping);
                Save();
            }
            else
            {
                epfEmployeeMapping.ModifiedDate = DateTime.Now;
                epfEmployeeMapping.IsActive = true;                
                Update(epfEmployeeMapping);
                Save();
            }

            for (int eidCount = 0; eidCount <= arrMappedId.Length - 1; eidCount = eidCount + 1)
            {
                int empId;
                if (int.TryParse(arrMappedId[eidCount], out empId))
                {
                    EpfemployeeMapping epfEmpMap = new EpfemployeeMapping();
                    epfEmpMap.EmployeeId = empId;
                    epfEmpMap.StatutoryComponentsId = epfEmployeeMapping.StatutoryComponentsId;
                    unMappedEmployeeMapDetailParam[eidCount] = epfEmpMap;
                }
            }
            //Delete Statuary
            ExecuteDelete<EpfemployeeMapping>();
            AddRange<EpfemployeeMapping>(unMappedEmployeeMapDetailParam);
            Save();
          //  trans.Commit();
        }
        catch (Exception ex)
        {
           // trans.Rollback();
            return "fail to save roster. Error occured while saving roster.";
        }


        return sMsg;
    }


}
