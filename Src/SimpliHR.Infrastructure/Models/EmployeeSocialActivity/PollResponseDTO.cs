using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.EmployeeSocialActivity;

public class PollResponseDTO
{
    
    public int PollResponseId { get; set; }

    
    public int EmployeeId { get; set; }

    
    public int QuestionId { get; set; }

    
    public int OptionId { get; set; }

    public DateTime CreatedDate { get; set; }
}
