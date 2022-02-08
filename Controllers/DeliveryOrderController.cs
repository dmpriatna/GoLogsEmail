using AutoMapper;
using GoLogs.Api.BusinessLogic.Interfaces;
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
    public class DeliveryOrderController : BaseController
    {
        private readonly IDeliveryOrderLogic _doLogic;
        private readonly IMapper _mapper;

        public DeliveryOrderController(
            IDeliveryOrderLogic doLogic,
            IMapper mapper)
        {
            _doLogic = doLogic;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetDOByJobNumber")]
        public async Task<ActionResult> GetDOByJobNumberAsync(string JobNumber)
        {
            try
            {
                return Ok(await _doLogic.GetDOByJobNumberAsync(JobNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
