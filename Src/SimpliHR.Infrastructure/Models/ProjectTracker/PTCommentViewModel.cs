using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTCommentViewModel
{
    public List<IFormFile>? Attachment { get; set; }
    public PTCommentDTO? Comments { get; set; }
}
