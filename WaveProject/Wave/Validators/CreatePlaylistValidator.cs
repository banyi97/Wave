using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Dtos;

namespace Wave.Validators
{
    public class CreatePlaylistValidator : AbstractValidator<CreatePlaylistDto>
    {
        public CreatePlaylistValidator()
        {
            RuleFor(q => q.Title).NotEmpty();
        }
    }
}
