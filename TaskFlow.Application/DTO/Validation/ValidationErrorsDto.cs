namespace TaskFlow.Application.DTO.Validation;

public class ValidationErrorsDto
{
    public string PropertyName { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}
