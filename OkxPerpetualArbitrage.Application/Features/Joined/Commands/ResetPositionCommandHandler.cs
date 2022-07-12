using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    /// <summary>
    /// Handles the request to fully reset a postion
    /// All the orders related to the symbol will be canceled and closed
    /// All the records of the symbol will be erased from the database
    /// This should only be used manually if there is a critical error 
    /// </summary>
    public class ResetPositionCommandHandler : IRequestHandler<ResetPositionCommand, ApiCommandResponseDto>
    {
        private readonly IResetPositionLogic _resetPositionLogic;

        public ResetPositionCommandHandler(IResetPositionLogic resetPositionLogic)
        {
            _resetPositionLogic = resetPositionLogic;
        }
        public async Task<ApiCommandResponseDto> Handle(ResetPositionCommand request, CancellationToken cancellationToken)
        {
            return await _resetPositionLogic.ResetPosition(request.Symbol, cancellationToken);
        }
    }
}
