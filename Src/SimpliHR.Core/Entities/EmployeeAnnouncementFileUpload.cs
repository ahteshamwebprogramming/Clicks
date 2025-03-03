using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("EmployeeAnnouncementFileUploads")]
public class EmployeeAnnouncementFileUpload
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeAnnouncementFileUploadsId { get; set; }

    public int EmployeeAnnouncementId { get; set; }

    public string? UploadedFileName { get; set; }

    public string? UploadedFilePath { get; set; }

    public string? UploadedFileExtension { get; set; }

    public string? UploadType { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
