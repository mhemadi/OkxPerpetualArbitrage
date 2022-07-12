using FluentValidation;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Validators.DTOs
{
    public class ClosePositionDtoValidator : AbstractValidator<ClosePositionDto>
    {
        public ClosePositionDtoValidator()
        {
            RuleFor(x => x.Size).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Symbol).NotEmpty();
        }
    }
}
