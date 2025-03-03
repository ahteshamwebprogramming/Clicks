using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.WebUI.Modals.Masters
{
    public class District
    {
        public int DistrictId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }

        //[StringLength(50)]
        //public string CityIds { get; set; } = null!;

        [StringLength(50)]
        public string? DistrictName { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public List<Country> Countries { get; set; }
        public List<State> States { get; set; }
        public List<District> Districts { get; set; }
    }
}
