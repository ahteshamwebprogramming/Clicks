using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class AttendanceMastersKeyValues
{
    public ICollection<DepartmentKeyValues> DepartmentKeyValues { get; set; } = new List<DepartmentKeyValues>();
    public ICollection<EmployeeKeyValues> EmployeeKeyValues { get; set; } = new List<EmployeeKeyValues>();
    public ICollection<ShiftKeyValues> ShiftKeyValues { get; set; } = new List<ShiftKeyValues>();
    public ICollection<WorkLocationKeyValues> WorkLocationKeyValues { get; set; } = new List<WorkLocationKeyValues>();
    public List<int> GetYears(int startYear, int noOfYears)
    {
        return Enumerable.Range(startYear, noOfYears).Reverse().ToList();
    }
    public Dictionary<int, string> GetMonth(int startMonth, int endMonth)
    {
        return  (Enumerable.Range(startMonth, endMonth).Select(i =>
                    new
                    {
                        MonthId = i,
                        MonthName = CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedMonthName(i)
                    })
                    .ToDictionary(x => x.MonthId, x => x.MonthName)
                );
    }

}
