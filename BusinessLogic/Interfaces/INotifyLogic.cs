using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface INotifyLogic
    {
        Task CreateNotifyAsync(Guid PersonId, string Subject, string Description);
        Task<List<NotifyModel>> GetNotifiesByPersonIdAsync(Guid PersonId);
        Task ReadNotifyAsync(NotifyCommand command);
        Task TransactionRequestAsync(string SelectedService, string JobNumber, Guid PersonId);
        Task TransactionInvoiceAsync(string SelectedService, string JobNumber, Guid PersonId);
        Task TransactionPaymentAsync(string SelectedService, string JobNumber, Guid PersonId);
        Task TransactionReleaseAsync(string SelectedService, string JobNumber, Guid PersonId);
    }
}
