using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Performace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Repository;

public interface IPerformanceKRAMasterDBRepository : IDapperRepository<PerformanceKRAMasterDB>
{
    Task<string> UploadKRADB(List<PerformanceKRAMasterDBDTO> data, int unitId, int PerformanceSettingId);
    Task<string> UploadBehavioralDB(List<PerformanceKRAMasterDBDTO> data, int unitId, int PerformanceSettingId);
}
