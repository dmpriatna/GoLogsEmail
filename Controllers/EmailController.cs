using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoLogs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : BaseController
    {
        private readonly IEmailLogic _emailLogic;
        private readonly IEmailTemplateLogic _emailTemplateLogic;

        public EmailController(
            IEmailLogic emailLogic,
            IEmailTemplateLogic emailTemplateLogic)
        {
            _emailLogic = emailLogic;
            _emailTemplateLogic = emailTemplateLogic;
        }

        [HttpGet]
        [Route("Test")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> Get(string email)
        {
            try
            {
                GlobalHelper.SendEmail(email, "tes email", "<b>tes body</b>");
                return new string[] { "Success", "Email sent." };
            }
            catch (Exception ex)
            {
                return new string[] { "Error", ex.Message };
            }
        }

        [HttpPost]
        [Route("AfterDORequest")]
        public async Task<ActionResult> AfterDORequestAsync([FromBody] EmailCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }

            if (string.IsNullOrEmpty(command.BLCode))
            {
                return BadRequest(Constant.ErrorFromServer + "BL Number is required");
            }

            try
            {
                await _emailLogic.AfterDORequestAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AfterInvoice")]
        public async Task<ActionResult> AfterInvoiceAsync([FromBody] EmailCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }

            try
            {
                await _emailLogic.AfterInvoiceAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AfterPayment")]
        public async Task<ActionResult> AfterPaymentAsync([FromBody] EmailCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }
			
			if (string.IsNullOrEmpty(command.BLCode))
            {
                return BadRequest(Constant.ErrorFromServer + "BL Number is required");
            }

            try
            {
                await _emailLogic.AfterPaymentAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AfterDORelease")]
        public async Task<ActionResult> AfterDOReleaseAsync([FromBody] EmailCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }

            try
            {
                await _emailLogic.AfterDOReleaseAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
		
		[HttpPost]
        [Route("AfterInvoiceKoja")]
		[AllowAnonymous]
        public async Task<ActionResult> AfterInvoiceKojaAsync([FromBody] EmailInvKojaCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }

            try
            {
                await _emailLogic.AfterInvoiceKojaAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
		
		[HttpPost]
        [Route("GatePass")]
		[AllowAnonymous]
        public async Task<ActionResult> GatePassAsync([FromBody] EmailGatePassKojaCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }

            try
            {
                await _emailLogic.GatePassAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Activation")]
        [AllowAnonymous]
        public async Task<ActionResult> ActivationAsync(string activationCode)
        {
            try
            {
                await _emailLogic.Activation(activationCode);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ResendActivation")]
        [AllowAnonymous]
        public async Task<ActionResult> ResendActivationAsync(string email)
        {
            try
            {
                await _emailLogic.ResendActivation(email);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateUpdateEmailTemplate")]
        public async Task<ActionResult> CreateUpdateEmailTemplateAsync([FromBody] EmailTemplateCommand command)
        {
            try
            {
                await _emailTemplateLogic.CreateUpdateEmailTemplateAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetEmailTemplateByType")]
        public async Task<ActionResult> GetEmailTemplateByTypeAsync(string type)
        {
            try
            {
                var result = await _emailTemplateLogic.GetEmailTemplateByTypeAsync(type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetEmailTemplate")]
        public async Task<ActionResult> GetEmailTemplateAsync()
        {
            try
            {
                var result = await _emailTemplateLogic.GetAllEmailTemplateAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
