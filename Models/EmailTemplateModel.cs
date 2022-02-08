using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public class EmailTemplateModel : BaseModel
    {
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Template { get; set; }
    }
}
