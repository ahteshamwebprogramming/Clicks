using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;

namespace SimpliHR.Infrastructure.Models.ClientManagement
{
    public partial class ClientDTO
    {

       
        public int ClientId { get; set; }

        //[DataType(DataType.Text)]
        //[Required(ErrorMessage = "Please enter Client"), MaxLength(50, ErrorMessage = "client cannot exceed 50 characters")]
        public string? ClientName { get; set; }
        public string? EncClientId { get; set; }

        //[DataType(DataType.Text)]
        //[Required(ErrorMessage = "Please enter Company Name"), MaxLength(200, ErrorMessage = "Company cannot exceed 200 characters")]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? GSTN { get; set; }


        //[DataType(DataType.EmailAddress)]
        //[Required(ErrorMessage = "Please enter Email ID")]
        public string? EmailId { get; set; }

        
        //[Required(ErrorMessage = "Please enter Contact Number"), MaxLength(10, ErrorMessage = "Contact Number cannot exceed 10 digits")]
        public long? ContactNumber { get; set; }

        //[DataType(DataType.Text)]
        //[Required(ErrorMessage = "Please enter the Address"), MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select country")]
        public int? CountryId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select state")]
        public int? StateId { get; set; }

        //[Column("DistrictID")]
        //public int? DistrictId { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select City")]
        public int? CityId { get; set; }

        //[Required(ErrorMessage = "Please enter Pincode"), MaxLength(6, ErrorMessage = "Pincode cannot exceed 10 digits")]
        public long? Pincode { get; set; }

        [StringLength(100)]
        public string? ClientLogo { get; set; }

        [StringLength(50)]
        public string? HeaderText { get; set; }

        [StringLength(500)]
        public string? FooterText { get; set; }

        [StringLength(500)]
        public string? SupportLink { get; set; }

        [StringLength(500)]
        public string? PoliciesLink { get; set; }

        [StringLength(500)]
        public string? DocumentLink { get; set; }

        [StringLength(5)]
        public string? MenuStyle { get; set; }

        [StringLength(100)]
        public string? ColorTheme { get; set; }

        public bool? IsActive { get; set; }

        public string? CountryName { get; set; }

        public string? StateName { get; set; }

        public string? CityName { get; set; }


        public string? CreatedBy { get; set; }
      //  [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
       // [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public List<CountryKeyValues> CountryList { get; set; }
        public List<StateKeyValues> StateList { get; set; }
        public List<CityKeyValues> CityList { get; set; }
        public CountryMasterDTO Country { get; set; } = null!;
        public StateMasterDTO State { get; set; } = null!;
        public CityMasterDTO City { get; set; } = null!;
        public List<IdtypeKeyValues> IDTypeList { get; set; }
        public IdtypeMasterDTO IDType { get; set; } = null!;
        public List<ModuleKeyValues> ModuleList { get; set; }
        public ModuleMasterDTO Module { get; set; } = null!;

        public ICollection<ClientDTO> ClientList { get; set; } = new List<ClientDTO>();
        public List<ClientDTO> lstClient { get; set; }
        public string DisplayMessage { get; set; } = "_blank";

        public string Base64ProfileImage { get; set; }


    }
}
