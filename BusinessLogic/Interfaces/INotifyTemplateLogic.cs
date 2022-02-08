using GoLogs.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface INotifyTemplateLogic
    {
        Task<NotifyTemplateViewModel> GetNotifyTemplateByTypeAsync(string type);
    }
}
