using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public class NotifyModel : BaseModel
    {
        public Guid PersonId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool ReadStatus { get; set; }
    }
}
