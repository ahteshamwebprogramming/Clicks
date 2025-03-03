using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Login
{
    public class AdminEncryption
    {
        public string EncryptInput { get; set; }
        public string EncryptOutput { get; set; }
        public string DecryptInput { get; set; }
        public string DecryptOutput { get; set; }


    }
}
