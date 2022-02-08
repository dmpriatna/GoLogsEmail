using AutoMapper;
using GoLogs.Api.Application.Internals;
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
    public class NotifyTemplateLogic : INotifyTemplateLogic
    {
        private readonly IMapper _mapper;
        private readonly GoLogsContext _context;
        public NotifyTemplateLogic(IMapper mapper, GoLogsContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<NotifyTemplateViewModel> GetNotifyTemplateByTypeAsync(string type)
        {
            var template = await _context.NotifyTemplates.Where(x => x.Type == type).FirstOrDefaultAsync();
            if (template == null)
            {
                throw new ArgumentException(Constant.ErrorFromServer + "Template does not exist.");
            }
            var templateView = _mapper.Map<NotifyTemplateModel, NotifyTemplateViewModel>(template);
            return templateView;
        }
    }
}
