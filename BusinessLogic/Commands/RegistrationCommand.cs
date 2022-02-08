using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Commands
{
    public class RegistrationCommand
    {
        public NLECustDataModel Company_Information { get; set; }
        public PersonInformationCommand Person_Information { get; set; }
    }

    public class PersonInformationCommand
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
