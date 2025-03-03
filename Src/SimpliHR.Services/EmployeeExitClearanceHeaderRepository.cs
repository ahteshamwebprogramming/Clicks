using Dapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Services.DBContext;
using System.Data;
using static Dapper.SqlMapper;
using System.Threading;

namespace SimpliHR.Services;

public class EmployeeExitClearanceHeaderRepository : DapperGenericRepository<EmployeeExitClearanceHeader>, IEmployeeExitClearanceHeaderRepository
{
    public EmployeeExitClearanceHeaderRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }

    public async Task<string> SaveExitClearanceInfo(List<EmployeeExitClearanceHeader> headerList, List<EmployeeExitClearanceDetail> detailList)
    {
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success", status;
        TimeSpan MinimumTime, MaximumTime;
        bool isEdit = false;
        string sQuery = string.Empty;
        ManualPunches manualPunch = new ManualPunches();
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                foreach (var item in headerList)
                {
                    if (item.EmployeeClearanceHeaderId == null || item.EmployeeClearanceHeaderId == 0)
                    {
                        item.CreatedOn = DateTime.Now;
                        item.IsActive = true;
                        item.EmployeeClearanceHeaderId = await ExecuteAddAsync(item, IDBConn, trans);
                    }
                    else
                    {
                        item.ModifiedOn = DateTime.Now;
                        item.IsActive = true;
                        await ExecuteUpdateAsync(item, IDBConn, trans);
                    }
                    //item.EmployeeClearanceHeaderId = await IDBConn.ExecuteAsync(sQuery, item, trans);

                    List<EmployeeExitClearanceDetail> itemDetails = detailList.Where(x => x.ClearanceMappingId == item.ClearanceMappingId).ToList();
                    foreach (var itemDetail in itemDetails)
                    {
                        if (itemDetail != null)
                        {
                            itemDetail.EmployeeClearanceHeaderId = item.EmployeeClearanceHeaderId;
                            if (!(itemDetail.EmployeeClearanceDetailId == null || itemDetail.EmployeeClearanceDetailId == 0))
                            {
                                sQuery = @"UPDATE EmployeeExitClearanceDetail set EmployeeClearanceHeaderId=@EmployeeClearanceHeaderId,AssetId=@AssetId,ClearanceMappingId=@ClearanceMappingId,AssetClearanceStatus=@AssetClearanceStatus,Remark=@Remark,RecoveryStatus=@RecoveryStatus,RecoveryAmount=@RecoveryAmount,IsActive=1,ModifiedOn=GETDATE()" +
                                " WHERE EmployeeClearanceDetailId=" + itemDetail.EmployeeClearanceDetailId;
                            }
                            else
                            {
                                sQuery = @"INSERT EmployeeExitClearanceDetail(EmployeeClearanceHeaderId,AssetId,ClearanceMappingId,AssetClearanceStatus,Remark,RecoveryStatus,RecoveryAmount,IsActive,CreatedOn)
                                VALUES(@EmployeeClearanceHeaderId,@AssetId,@ClearanceMappingId,@AssetClearanceStatus,@Remark,@RecoveryStatus,@RecoveryAmount,1,GETDATE())";
                            }
                            await IDBConn.ExecuteAsync(sQuery, itemDetail, trans);
                        }
                    }
                }
                trans.Commit();

            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return "fail to save Employee clearance Info.";
        }

        return "Success";
    }

}


