using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PTTask")]
public class PTTask
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TaskID { get; set; }

    public string TaskDescription { get; set; } = null!;

    public int AssignedTo { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public int StatusID { get; set; }

    public string? Priority { get; set; }

    public int ProjectID { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    [DefaultValue(true)]
    public bool? IsActive { get; set; }

    
}
