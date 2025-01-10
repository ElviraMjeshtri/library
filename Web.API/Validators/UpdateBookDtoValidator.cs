using FluentValidation;
using Web.API.DTOs;
using Web.API.Models.DTOs;

namespace Web.API.Validators;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(b => b.Isbn)
             .Matches(@"^[0-9]{13}$")
             .WithMessage("ISBN should contain 13 digits");
        RuleFor(b => b.Title)
            .NotEmpty()
            .WithMessage("Title is required");
        RuleFor(b => b.Author)
            .NotEmpty()
            .WithMessage("Author is required");
        RuleFor(b => b.ShortDescription)
            .NotEmpty()
            .WithMessage("ShortDescription is required");
        RuleFor(b => b.PageCount)
            .GreaterThan(0)
            .WithMessage("PageCount is required");
    }
}