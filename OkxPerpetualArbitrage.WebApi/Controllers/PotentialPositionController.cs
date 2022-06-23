using MediatR;
using Microsoft.AspNetCore.Mvc;
using OkxPerpetualArbitrage.Application.Features.Joined.Commands;
using OkxPerpetualArbitrage.Application.Features.Joined.Queries;
using OkxPerpetualArbitrage.Application.Features.PotentialPositions.Commands;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.WebApi.Controllers
{
    public class PotentialPositionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PotentialPositionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/potentialposition")]
        [HttpGet]
        public async Task<IActionResult> GetPotentialPositions()
        {
            var dto = await _mediator.Send(new GetPotentialPositionsQuery());
            return Ok(dto);
        }
        [Route("api/potentialposition/Symbol")]
        [HttpGet]
        public async Task<IActionResult> GetSymbols()
        {
            var dto = await _mediator.Send(new GetSymolsQuery());
            return Ok(dto);
        }

        [Route("api/potentialposition/{symbol}/openData")]
        [HttpGet]
        public async Task<IActionResult> GetPPCloseData(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol can not be empty");
            var dto = await _mediator.Send(new GetPositionOpenDataQuery() { Symbol = symbol });
            return Ok(dto);
        }

        [Route("api/potentialposition/{symbol}/open")]
        [HttpPost]
        public async Task<IActionResult> OpenPosition(string symbol, [FromBody] OpenPositionDto openPositionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _mediator.Send(new OpenPositionCommand() { OpenPositionDto = openPositionDto }));
        }
    }
}
