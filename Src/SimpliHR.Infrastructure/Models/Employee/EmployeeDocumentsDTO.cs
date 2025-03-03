using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee
{
    public partial class EmployeeDocumentsDTO
    {
        public int UploadDcumentDetailId { get; set; }
        public int EmployeeId { get; set; }
        public string? TicketId { get; set; }
        public string? EntrySource { get; set; }
        public string? TableReferenceId { get; set; }
        public string? EmailId { get; set; }
        [MaxLength]
        public List<byte[]>? UploadedDocs { get; set; }
        public IFormFile[] UploadedFile { get; set; }
        public string? FieldName { get; set; }
        public string? ChangeValue { get; set; }
        public string FormName { get; set; }
        public int? ClientId { get; set; }
    }
}
