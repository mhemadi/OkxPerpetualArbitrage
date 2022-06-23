using MediatR;
using Microsoft.AspNetCore.Mvc;
using OkxPerpetualArbitrage.Application.Features.PositionDemands.Commands;
using OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.WebApi.Controllers
{
    public class PositionDemandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PositionDemandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/positiondemand/{symbol}/status")]
        [HttpGet]
        public async Task<IActionResult> GetDemandStatus(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol can not be empty");
            var dto = await _mediator.Send(new GetPositionDemandStatusQuery() { Symbol = symbol });
            return Ok(dto);

        }

        [Route("api/positiondemand/{demandId}/cancel")]
        [HttpPost]
        public async Task<IActionResult> CancelDemand(int demandId)
        {
            if (demandId <= 0)
                return BadRequest("DemandId can not be less than 0");
            return Ok(await _mediator.Send(new CancelDemandCommand() { DemandId = demandId }));
        }
    }
}
