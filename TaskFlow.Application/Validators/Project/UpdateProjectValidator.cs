using FluentValidation;
using TaskFlow.Application.DTO.Project;

namespace TaskFlow.Application.Validators.Project;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectRequestDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project Id is required!");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required!")
            .MinimumLength(3)
            .WithMessage("Title must be at least 3 characters!")
            .MaximumLength(35)
            .WithMessage("Title can't be more than 35 characters!");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required!")
            .MaximumLength(1000)
            .WithMessage("Description can't be more than 1000 characters!");
    }
}
