using FluentValidation;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Validators.TaskItem;

public sealed class CreateTaskItemValidator : AbstractValidator<AddTaskItemRequestDto>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(100)
            .WithMessage("Maximum Length of Title is 100 characters!")
            .MinimumLength(3)
            .WithMessage("Maximum Length of Title is 3 characters!");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required!")
            .MaximumLength(1000)
            .WithMessage("Maximum Length of Description is 1000 characters!");

        RuleFor(x => x.Status)
            .Must(x => Enum.IsDefined(typeof(Domain.Enums.TaskStatus), x))
            .WithMessage("Invalid status!");

        RuleFor(x => x.Priority)
            .Must(x => Enum.IsDefined(typeof(TaskPriority), x))
            .WithMessage("Invalid priority!");

        RuleFor(x => x.DeadLine)
            .Must(x => x > DateTime.Now)
            .WithMessage("Can't make a deadline for the past!");

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project is required!");

        RuleFor(x => x.AssignedToUserId)
            .NotEmpty()
            .WithMessage("Task must assigned to a user!");
    }
}
