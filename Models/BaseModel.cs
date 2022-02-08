using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public class BaseModel : IBaseModel
    {
        [Key]
        public Guid Id { get; set; }

        public bool RowStatus { get; set; }

        public Guid CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid ModifiedById { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
