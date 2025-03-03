using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PollResponses")]
public class PollResponse
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PollResponsesId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int OptionId { get; set; }

    public DateTime CreatedDate { get; set; }
}
