using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoLogs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : BaseController
    {
        private readonly INotifyLogic _notifyLogic;

        public NotifyController(
            INotifyLogic notifyLogic)
        {
            _notifyLogic = notifyLogic;
        }

        [HttpPost]
        [Route("ReadNotify")]
        public async Task<ActionResult> ReadNotifyAsync([FromBody] NotifyCommand command)
        {
            if (command == null)
            {
                return BadRequest(Constant.ErrorFromServer + "Invalid client request");
            }

            try
            {
                await _notifyLogic.ReadNotifyAsync(command);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetNotifiesByPersonId")]
        public async Task<ActionResult> GetNotifiesByPersonIdAsync(Guid PersonId)
        {
            try
            {
                var result = await _notifyLogic.GetNotifiesByPersonIdAsync(PersonId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
