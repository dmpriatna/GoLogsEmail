using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoLogs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IPersonLogic _personLogic;
        private readonly IMapper _mapper;

        public AuthController(
            IPersonLogic personLogic,
            IMapper mapper)
        {
            _personLogic = personLogic;
            _mapper = mapper;
        }

        // GET api/values
        [HttpPost, Route("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthModel model)
        {
            if (model == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request.");
            }

            var person = await _personLogic.GetPersonByEmailAsync(model.Email);

            if (person == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid Email.");
            }
            else if (!person.Activated)
            {
                return BadRequest(Constant.ErrorFromServer + "This customer has not been activated. Please check your email or resend activation url.");
            }
            else
            {
                if (model.Password == GlobalHelper.Decrypt(person.PasswordHash))
                {
                    var personView = await _personLogic.GetPersonViewByIdAsync(person.Id);
                    var token = AuthHelper.JWTAuth(personView);
                    token.Person = personView;
                    return Ok(token);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }
    }
}
