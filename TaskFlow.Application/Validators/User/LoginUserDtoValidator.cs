using FluentValidation;
using TaskFlow.Application.DTO.User;

namespace TaskFlow.Application.Validators.User;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required!")
            .EmailAddress()
            .WithMessage("Email is incorrect!");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required!");
    }
}
