using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Interfaces
{
    public interface IEmailTemplateLogic
    {
        Task CreateUpdateEmailTemplateAsync(EmailTemplateCommand model);
        Task<EmailTemplateViewModel> GetEmailTemplateByTypeAsync(string type);
        Task<List<EmailTemplateModel>> GetAllEmailTemplateAsync();
    }
}
