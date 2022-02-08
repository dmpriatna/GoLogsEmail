using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public class NotifyTemplateModel : BaseModel
    {
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
