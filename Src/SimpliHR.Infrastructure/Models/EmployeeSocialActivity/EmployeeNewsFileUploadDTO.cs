using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.EmployeeSocialActivity;

public class EmployeeNewsFileUploadDTO
{
    public int EmployeeNewsFileUploadsId { get; set; }

    public int EmployeeNewsId { get; set; }

    public string? UploadedFileName { get; set; }

    public string? UploadedFilePath { get; set; }

    public string? UploadedFileExtension { get; set; }

    public string? UploadType { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
