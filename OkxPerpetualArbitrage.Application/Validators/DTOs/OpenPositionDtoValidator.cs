using FluentValidation;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Validators.DTOs
{
    internal class OpenPositionDtoValidator : AbstractValidator<OpenPositionDto>
    {
        public OpenPositionDtoValidator()
        {
            RuleFor(x => x.Size).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Symbol).NotEmpty();
        }
    }
}
