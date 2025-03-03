using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Exit;

public class EmployeeExitManagementViewModel
{
    public EmployeeExitInterViewFormMasterDTO? ExitInterviewForm { get; set; }
    public List<EmployeeExitResignationDTO>? ExitResignationList { get; set; }

    public List<component>? HeaderComponents { get; set; }
    public List<InterviewResponses>? ResponseComponent { get; set; }

    public InterviewResponses InterviewFilters { get; set; }
}



public class InterviewResponses
{
    public int UnitId { get; set; }
    public DateTime? LastWorkingDateFrom { get; set; }
    public DateTime? LastWorkingDateTo { get; set; }
    public EmployeeExitResignationDTO? ResignationDetails { get; set; }

    public List<component>? Responses { get; set; }
}



public class component 
{
    public string? label { get; set; }
    public string? tableView { get; set; }
    public string? key { get; set; }
    public string? type { get; set; }
    public bool? input { get; set; }
    public List<question>? questions { get; set; }
    public List<valuee>? values { get; set; }
    public string? response { get; set; }
    
}
public class question
{
    public string? label { get; set; }
    public string? value { get; set; }
    public string? tooltip { get; set; }
    public string response { get; set; }
}
public class valuee
{
    public string? label { get; set; }
    public string? value { get; set; }
    public string? shortcut { get; set; }
}