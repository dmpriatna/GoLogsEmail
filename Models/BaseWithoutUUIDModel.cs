using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace GoLogs.Api.Models
{
    public class BaseWithoutUUIDModel
    {
        [Key]
        public Guid Id { get; set; }

        public int RowStatus { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
