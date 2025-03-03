using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.EmployeeSocialActivity;


public class SurveyPollsQuestionDTO
{
    public int SurveyPollQuestionId { get; set; }

    public int EmployeeAnnouncementId { get; set; }

    public string? Question { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int? Response { get; set; }
    public int? TotalVotes { get; set; }
}
