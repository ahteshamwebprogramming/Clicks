using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceEmployeeDataDTO
{
    public int PerformanceEmployeeDataId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int PerformanceSettingId { get; set; }

    public double? KRAWeightageTotal { get; set; }

    public double? KRAEmployeeRatingTotal { get; set; }

    public double? KRAManagersRatingTotal { get; set; }

    public double? BehaviouralSkillsWeightageTotal { get; set; }

    public double? BehaviouralSkillsEmployeeRatingTotal { get; set; }

    public double? BehaviouralSkillsManagersRatingTotal { get; set; }

    public string? ClosingRemarksEmployee { get; set; }

    public string? ClosingRemarksManager { get; set; }

    public double? RatingCalculationKRAWeightage { get; set; }

    public double? RatingCalculationKRAScore { get; set; }

    public double? RatingCalculationKRAFinalScore { get; set; }

    public double? RatingCalculationBehaviouralSkillsWeightage { get; set; }

    public double? RatingCalculationBehaviouralSkillsScore { get; set; }

    public double? RatingCalculationBehaviouralSkillsFinalScore { get; set; }

    public double? RatingCalculationFinalScore { get; set; }

    public string? RatingCalculationFinalRating { get; set; }

    public int? RatingCalculationFinalRatingId { get; set; }

    public string? HODFinalRating { get; set; }

    public int? HODFinalRatingId { get; set; }

    public string? ClosingRemarksHOD { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDateEmployee { get; set; }

    public DateTime? ModifiedDateManager { get; set; }

    public DateTime? ModifiedDateHOD { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedByEmployee { get; set; }

    public int? ModifiedByManager { get; set; }

    public int? ModifiedByHOD { get; set; }

    public bool? FilledByEmployee { get; set; }

    public bool? FilledByManager { get; set; }

    public bool? FilledByHOD { get; set; }
    public float? KRAScoreTotal { get; set; }
    public float? BehaviouralSkillsScoreTotal { get; set; }
    public string ViewType { get; set; }
    public bool Published { get; set; }

}
