using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.EmployeeSocialActivity;

public class EmployeeNewsDTO
{
    public int EmployeeNewsId { get; set; }
    public string? encEmployeeNewsId { get; set; }

    public int UnitId { get; set; }

    public string Title { get; set; } = null!;

    public string? Article { get; set; }
    public int? ArticleType { get; set; }
    public string? Description { get; set; }

    public string? ArticleLink { get; set; }

    public string AuthorsNameRaw { get; set; } = null!;

    public string AuthorsName { get; set; } = null!;

    public string PublicationName { get; set; } = null!;

    public string? TaggingRaw { get; set; } = null!;
    public string? CategoryTags { get; set; } = null!;

    public string Tagging { get; set; } = null!;

    public string KeywordsRaw { get; set; } = null!;

    public string Keywords { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
    public string? ImagePath { get; set; }

    public string? Source { get; set; }
}
