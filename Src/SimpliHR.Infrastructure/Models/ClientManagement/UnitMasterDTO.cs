using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ClientManagement
{
    public partial class UnitMasterDTO
    {

        public int UnitID { get; set; }

        public int ClientId { get; set; }

        public string? UnitName { get; set; }
        public string? EmailDisplayName { get; set; }

        public string EunitID { get; set; }
        public string? GSTN { get; set; }
        public string? TIN { get; set; }
        public string? PanCard { get; set; }

        public string? EmailId { get; set; }

        public long? ContactNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? Address { get; set; }

        public int? CountryId { get; set; }

        public int? StateId { get; set; }

        public int? CityId { get; set; }

        public string? CountryName { get; set; }

        public string? StateName { get; set; }

        public string? CityName { get; set; }
        public string? ClientName { get; set; }

        public long? Pincode { get; set; }
        public int? NoticePeriod { get; set; }
        public int? ConfirmationPeriod { get; set; }

        public string? WeeklyOff { get; set; }

        public int? PayrollStartDate { get; set; }

        public int? PayrollEndDate { get; set; }
        public bool? IsActive { get; set; }

        public int? IsBlock { get; set; }
        public bool? AttendaceSandwichRule { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public string Base64ProfileImage { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public HttpResponseMessage? HttpMessage { get; set; }
        public List<CountryKeyValues> CountryList { get; set; }
        public List<StateKeyValues> StateList { get; set; }
        public List<CityKeyValues> CityList { get; set; }

        public List<ClientKeyValues> ClientList { get; set; }
        
        public CountryMasterDTO Country { get; set; } = null!;
        public StateMasterDTO State { get; set; } = null!;
        public CityMasterDTO City { get; set; } = null!;
        public ClientDTO Client { get; set; } = null!;

        public virtual ICollection<ClientModuleMappingDTO> ClientModuleMappings { get; set; } = new List<ClientModuleMappingDTO>();

        public ICollection<UnitMasterDTO> UnitMasterList { get; set; } = new List<UnitMasterDTO>();
        public List<UnitMasterDTO> lstUnitMaster { get; set; }
        public string DisplayMessage { get; set; } = "_blank";


        

        

        

        

        

        


    }
}
