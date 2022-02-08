using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Handler
{
    public class NotifyLogic : INotifyLogic
    {
        private readonly GoLogsContext _context;
        private readonly IDeliveryOrderLogic _doLogic;
        private readonly IMapper _mapper;
        private readonly INotifyTemplateLogic _notifyTemplate; 

        public NotifyLogic(
            GoLogsContext context,
            IDeliveryOrderLogic doLogic,
            IMapper mapper,
            INotifyTemplateLogic notifyTemplate)
        {
            _context = context;
            _doLogic = doLogic;
            _mapper = mapper;
            _notifyTemplate = notifyTemplate;
        }

        public async Task CreateNotifyAsync(Guid PersonId, string Subject, string Description)
        {
            var notify = new NotifyModel()
            {
                Id = Guid.NewGuid(),
                PersonId = PersonId,
                Subject = Subject,
                Description = Description
            };
            _context.Notifies.Add(notify);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotifyModel>> GetNotifiesByPersonIdAsync(Guid PersonId)
        {
            var notifies = await _context.Notifies.Where(x => x.PersonId == PersonId)
                .OrderByDescending(x => x.ModifiedDate).ToListAsync();
            return notifies;
        }

        public async Task ReadNotifyAsync(NotifyCommand command)
        {
            var notifies = _context.Notifies.Where(x => command.Id.Contains(x.Id));
            foreach (var item in notifies)
            {
                item.ReadStatus = true;
                _context.Notifies.Update(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task TransactionRequestAsync(string SelectedService, string JobNumber, Guid PersonId)
        {
            var template = await _notifyTemplate.GetNotifyTemplateByTypeAsync("TransactionRequestCustomer");
            var subject = template.Subject.Replace("@SelectedService", SelectedService)
                .Replace("@JobNumber", JobNumber);
            var description = template.Description.Replace("@JobNumber", JobNumber);
            await CreateNotifyAsync(PersonId, subject, description);
        }

        public async Task TransactionInvoiceAsync(string SelectedService, string JobNumber, Guid PersonId)
        {
            var template = await _notifyTemplate.GetNotifyTemplateByTypeAsync("TransactionInvoice");
            var subject = template.Subject.Replace("@SelectedService", SelectedService)
                .Replace("@JobNumber", JobNumber);
            var description = template.Description.Replace("@JobNumber", JobNumber);
            await CreateNotifyAsync(PersonId, subject, description);
        }

        public async Task TransactionPaymentAsync(string SelectedService, string JobNumber, Guid PersonId)
        {
            var template = await _notifyTemplate.GetNotifyTemplateByTypeAsync("TransactionPaymentCustomer");
            var subject = template.Subject.Replace("@SelectedService", SelectedService)
                .Replace("@JobNumber", JobNumber);
            var description = template.Description.Replace("@JobNumber", JobNumber);
            await CreateNotifyAsync(PersonId, subject, description);
        }

        public async Task TransactionReleaseAsync(string SelectedService, string JobNumber, Guid PersonId)
        {
            var template = await _notifyTemplate.GetNotifyTemplateByTypeAsync("TransactionRelease");
            var subject = template.Subject.Replace("@SelectedService", SelectedService)
                .Replace("@JobNumber", JobNumber);
            var description = template.Description.Replace("@JobNumber", JobNumber);
            await CreateNotifyAsync(PersonId, subject, description);
        }
    }
}
