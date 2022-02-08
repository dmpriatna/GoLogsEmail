using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface INLELogic
    {
        Task<NLEAuthModel> GetAuthTokenAsync();

        Task<NLECustDataViewModel> GetCustomerDataProfileOSSAsync(string npwp);

        Task RegistrationAsync(RegistrationCommand command);
    }
}
