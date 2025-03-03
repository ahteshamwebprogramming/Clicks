using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.Infrastructure.Models.Attendance;

public partial class WorkFlowSettingsDTO
    {

    public string EncryptedId { get; set; }
    public int WorkFlowSettingsId { get; set; }
    public string? Name { get; set; }

    public int? ActionId { get; set; }

    public int? ModuleId { get; set; }

   // public int? LevelId1 { get; set; }

    public int? Authority1 { get; set; }
   // public int? LevelId2 { get; set; }

    public int? Authority2 { get; set; }

   // public int? LevelId3 { get; set; }

    public int? Authority3 { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }
    public string? ModuleName { get; set; }

    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }
       
    public DateTime? ModifiedOn { get; set; }

    public int? ClientId { get; set; }

    public int? UnitId { get; set; }
    public List<ModuleKeyValues>? ModuleMasterList { get; set; }
    public List<WorkFlowSettingsDTO>? WorkFlowSettingsList { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
    public int? HttpStatusCode { get; set; } = 200;
    public ModuleMasterDTO Module { get; set; } = null!;

}

