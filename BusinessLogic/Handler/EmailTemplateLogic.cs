using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Handler
{
    public class EmailTemplateLogic : IEmailTemplateLogic
    {
        private readonly IMapper _mapper;
        private readonly GoLogsContext _context;
        public EmailTemplateLogic(IMapper mapper, GoLogsContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task CreateUpdateEmailTemplateAsync(EmailTemplateCommand command)
        {
            var template = await _context.EmailTemplates
                .Where(x => x.Type == command.Type).FirstOrDefaultAsync();

            if (template == null)
            {
                var model = _mapper.Map<EmailTemplateCommand, EmailTemplateModel>(command);
                _context.EmailTemplates.Add(model);
                await _context.SaveChangesAsync();
            }
            else
            {
                // throw new ArgumentException(Constant.ErrorFromServer + "This template is already exist.");
                template.Subject = command.Subject;
                template.Template = command.Template;
                _context.EmailTemplates.Update(template);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EmailTemplateViewModel> GetEmailTemplateByTypeAsync(string type)
        {
            var template = await _context.EmailTemplates.Where(x => x.Type == type).FirstOrDefaultAsync();
            if (template == null)
            {
                throw new ArgumentException(Constant.ErrorFromServer + "Template does not exist.");
            }
            var templateView = _mapper.Map<EmailTemplateModel, EmailTemplateViewModel>(template);
            return templateView;
        }

        public async Task<List<EmailTemplateModel>> GetAllEmailTemplateAsync()
        {
            var templates = await _context.EmailTemplates.ToListAsync();
            return templates;
        }
    }
}
