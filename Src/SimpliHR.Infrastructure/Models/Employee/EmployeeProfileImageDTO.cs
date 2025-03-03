using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee
{
    public partial class EmployeeProfileImageDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }
        public IFormFile ProfileImageFile { get; set; }
        public string? CroppedImageBase64 { get; set; }
        public string? ProfileImageExtension { get; set; }
        public int? ClientId { get; set; }
        public int? UnitId { get; set; }
        public string FormName { get; set; }
    }
}
