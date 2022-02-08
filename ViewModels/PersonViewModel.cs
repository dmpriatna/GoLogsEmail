using GoLogs.Api.Models;

namespace GoLogs.Api.ViewModels
{
    public class PersonViewModel : BaseModel
    {

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public bool Activated { get; set; }

        public CompanyModel Company { get; set; }
    }
}
