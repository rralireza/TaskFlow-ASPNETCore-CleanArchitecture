using FluentValidation;
using TaskFlow.Application.DTO.User;

namespace TaskFlow.Application.Validators.User;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .WithMessage("Fullname field is required!")
            .MaximumLength(90)
            .WithMessage("Fullname can't be more than 90")
            .MinimumLength(3)
            .WithMessage("Fullname can't be less than 3");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required!")
            .EmailAddress()
            .WithMessage("Email format is incorrect!");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required!")
            .MinimumLength(6)
            .WithMessage("Password length must be more than 6 characters!")
            .MaximumLength(64)
            .WithMessage("Password length can't be more than 64 characters!");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("User must have a role!");
    }
}
