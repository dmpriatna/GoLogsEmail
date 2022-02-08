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
    public class DeliveryOrderLogic : IDeliveryOrderLogic
    {
        private readonly IMapper _mapper;
        private readonly GoLogsContext _context;
        public DeliveryOrderLogic(IMapper mapper, GoLogsContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<DeliveryOrderViewModel> GetDOByJobNumberAsync(string JobNumber)
        {
            var DO = await _context.DeliveryOrders.Where(x => x.JobNumber == JobNumber).FirstOrDefaultAsync();
            if (DO == null)
            {
                throw new ArgumentException(Constant.ErrorFromServer + "Delivery Order does not exist.");
            }

            var DOView = _mapper.Map<DeliveryOrderModel, DeliveryOrderViewModel>(DO);

            var person = await _context.Persons.Where(x => x.Id == DOView.CustomerID).FirstOrDefaultAsync();
            if (person == null)
            {
                throw new ArgumentException(Constant.ErrorFromServer + "Customer does not exist.");
            }

            var personView = _mapper.Map<PersonModel, PersonViewModel>(person);
            DOView.PersonData = personView;

            var doContainer = await _context.DeliveryOrderContainers.Where(x => x.DeliveryOrderId == DO.Id).ToListAsync();
            DOView.DOContainerData = doContainer;

            return DOView;
        }
    }
}
