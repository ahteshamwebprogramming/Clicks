using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SimpliHR.WebUI.Masters;

namespace SimpliHR.WebUI.Modals.Masters
{
    public class City
    {
        [Column("CityID")]
        public int CityId { get; set; }

        [Column("CountryID")]
        public int CountryId { get; set; }

        [Column("StateID")]
        public int StateId { get; set; }

        [StringLength(50)]
        public string CityName { get; set; } = null!;

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        [Column("modifiedOn", TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

        public List<City> Cities { get; set; }
        public List<District> Districts { get; set; }
        public List<State> States { get; set; }
        public List<Country> Countries { get; set; }

    }
}
