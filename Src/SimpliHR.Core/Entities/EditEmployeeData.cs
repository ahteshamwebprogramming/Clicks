using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("EditEmployeeData")]
public class EditEmployeeData
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeUpdateId { get; set; }

    public int? EmployeeId { get; set; }

    public string? ChangeType { get; set; }

    public string? ChangeValue { get; set; }

    public DateTime? Wefdate { get; set; }

    public string? OldValue { get; set; }

    public int? IsApproved { get; set; }
    
    public int? ApprovedBy { get; set; }   
    
    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }
    
    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsActive { get; set; }

    [MaxLength]
    public byte[]? Attachment { get; set; }
    public string? DocumentType { get; set; }
    public int? EmployeeValidationId { get; set; }
    public int? TableReferenceId { get; set; }
}
