using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Dtos;

namespace Wave.Validators
{
    public class CreateArtistValidator : AbstractValidator<CreateArtistDto>
    {
        public CreateArtistValidator()
        {
            RuleFor(q => q.Name).NotEmpty();
            RuleFor(q => q.Description).NotEmpty();
        }
    }
}
