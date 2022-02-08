using GoLogs.Api.ViewModels;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface IDeliveryOrderLogic
    {
        Task<DeliveryOrderViewModel> GetDOByJobNumberAsync(string JobNumber);
    }
}
