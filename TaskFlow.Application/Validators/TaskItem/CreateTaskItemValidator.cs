using FluentValidation;
using TaskFlow.Application.DTO.TaskItem;

namespace TaskFlow.Application.Validators.TaskItem;

public sealed class CreateTaskItemValidator : AbstractValidator<AddTaskItemRequestDto>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(50)
            .WithMessage("Maximum Length of Title is 50 characters!");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required!");
    }
}
