using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Models
{
    public class PersonModel : BaseModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Phone { get; set; }

        public string NPWP { get; set; }

        public Guid ActivationCode { get; set; }

        public bool Activated { get; set; }

        public Guid CompanyId { get; set; }
    }
}
