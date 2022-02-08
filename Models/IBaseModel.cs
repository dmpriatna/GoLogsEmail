using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public interface IBaseModel
    {
        [Key]
        Guid Id { get; set; }

        bool RowStatus { get; set; }

        Guid CreatedById { get; set; }

        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        Guid ModifiedById { get; set; }

        string ModifiedBy { get; set; }

        DateTime ModifiedDate { get; set; }
    }
}
