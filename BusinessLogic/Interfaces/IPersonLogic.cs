using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using System;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface IPersonLogic
    {
        Task<PersonModel> GetPersonByIdAsync(Guid id);

        Task<PersonViewModel> GetPersonViewByIdAsync(Guid id);

        Task<PersonModel> GetPersonByEmailAsync(string email);

        Task<PersonModel> GetPersonByNPWPAsync(string npwp);

        Task<PersonModel> GetPersonByActivationCodeAsync(string ActivationCode);
    }
}
