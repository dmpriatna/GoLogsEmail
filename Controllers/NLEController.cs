using System;
using System.Threading.Tasks;
using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using GoLogs.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoLogs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NLEController : ControllerBase
    {
        private readonly INLELogic _nleLogic;
        private readonly IMapper _mapper;
        private readonly IPersonLogic _personLogic;

        public NLEController(
            INLELogic nleLogic,
            IMapper mapper,
            IPersonLogic personLogic)
        {
            _nleLogic = nleLogic;
            _mapper = mapper;
            _personLogic = personLogic;
        }

        [HttpGet]
        [Route("GetAuthToken")]
        public async Task<ActionResult> GetAuthTokenAsync()
        {
            return Ok(await _nleLogic.GetAuthTokenAsync());
        }

        [HttpGet]
        [Route("GetCustomer")]
        [ActionName("GetCustomer")]
        public async Task<ActionResult> GetCustomerAsync(string npwp, string email)
        {
            try
            {
                var NLECustData = await _nleLogic.GetCustomerDataProfileOSSAsync(npwp);
                var person = new PersonModel();
                person = await _personLogic.GetPersonByNPWPAsync(npwp);
                person = person == null ? await _personLogic.GetPersonByEmailAsync(email) : person;

                if (NLECustData == null)
                {
                    if (person == null)
                    {
                        return NotFound();
                    }
                }
                else
                {
                    if (person == null)
                    {
                        return Ok(NLECustData);
                    }
                }

                if (!person.Activated)
                {
                    return BadRequest(Constant.ErrorFromServer + "This customer has not been activated. Please check your email or resend activation url.");
                }

                var personView = await _personLogic.GetPersonViewByIdAsync(person.Id);
                var token = AuthHelper.JWTAuth(personView);
                token.Person = personView;
                return Ok(token);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<ActionResult> RegistrationAsync([FromBody] RegistrationCommand command)
        {
            try
            {
                await _nleLogic.RegistrationAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
