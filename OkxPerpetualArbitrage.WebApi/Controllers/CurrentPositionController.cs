using MediatR;
using Microsoft.AspNetCore.Mvc;
using OkxPerpetualArbitrage.Application.Features.Joined.Commands;
using OkxPerpetualArbitrage.Application.Features.Joined.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.WebApi.Controllers
{
    public class CurrentPositionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrentPositionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/currentposition")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentPositions()
        {
            var dto = await _mediator.Send(new GetCurrentPositionsQuery() { CheckError = false });
            return Ok(dto);
        }
        [Route("api/currentpositioncheckerror")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentPositionsCheckError()
        {
            var dto = await _mediator.Send(new GetCurrentPositionsQuery() { CheckError = true });
            return Ok(dto);
        }
        [Route("api/currentposition/{symbol}/closedata")]
        [HttpGet]
        public async Task<IActionResult> GetPPCloseData(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol can not be empty");
            var dto = await _mediator.Send(new GetPositionCloseDataQuery() { Symbol = symbol });
            return Ok(dto);
        }


        [Route("api/currentposition/{symbol}/close")]
        [HttpPost]
        public async Task<IActionResult> ClosePosition(string symbol, [FromBody] ClosePositionDto closePositionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _mediator.Send(new ClosePositionCommand() { ClosePositionDto = closePositionDto }));
        }


        [Route("api/currentposition/{symbol}/reset")]
        [HttpPost]
        public async Task<IActionResult> ResetPosition(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol can not be empty");
            return Ok(await _mediator.Send(new ResetPositionCommand() { Symbol = symbol }));
        }

    }
}
