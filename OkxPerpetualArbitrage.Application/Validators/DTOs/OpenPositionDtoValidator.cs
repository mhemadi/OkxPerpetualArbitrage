using FluentValidation;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
