using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.ViewModels
{
    public class AuthViewModel
    {
        public string AccessToken { get; set; }
        
        public string TokenType { get; set; } = "Bearer";

        public DateTime Issued { get; set; }

        public DateTime Expires { get; set; }

        public object Person { get; set; }
    }
}
