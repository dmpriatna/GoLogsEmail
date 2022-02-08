using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.BusinessLogic.Handler
{
    public class PersonLogic : IPersonLogic
    {
        private readonly GoLogsContext _context;
        private readonly IMapper _mapper;

        public PersonLogic(GoLogsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PersonModel> GetPersonByIdAsync(Guid id)
        {
            var person = await _context.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();
            return person;
        }
        public async Task<PersonViewModel> GetPersonViewByIdAsync(Guid id)
        {
            var person = await _context.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();
            var company = await _context.Companies.Where(x => x.Id == person.CompanyId).FirstOrDefaultAsync();
            var personView = _mapper.Map<PersonModel, PersonViewModel>(person);
            personView.Company = company;
            return personView;
        }

        public async Task<PersonModel> GetPersonByEmailAsync(string email)
        {
            var person = await _context.Persons.Where(x => x.Email == email).FirstOrDefaultAsync();
            return person;
        }

        public async Task<PersonModel> GetPersonByNPWPAsync(string npwp)
        {
            var person = await _context.Persons.Where(x => x.NPWP == npwp).FirstOrDefaultAsync();
            return person;
        }

        public async Task<PersonModel> GetPersonByActivationCodeAsync(string ActivationCode)
        {
            var guidCode = Guid.Parse(ActivationCode);
            var person = await _context.Persons.Where(x => x.ActivationCode == guidCode).FirstOrDefaultAsync();
            return person;
        }
    }
}
