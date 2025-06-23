using FluentValidation;
using FluentValidation.Results;
using TaskFlow.Application.DTO.User;
using TaskFlow.Application.Helpers;
using TaskFlow.Application.Intefaces.Services.JWT;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Users;

public class UserAdderService : IUserAdderService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IValidator<CreateUserDto> _validator;
    private readonly IValidator<LoginUserDto> _loginValidator;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserAdderService(IUnitOfWork unitOfWork, IValidator<CreateUserDto> validator, IJwtTokenGenerator jwtTokenGenerator, IValidator<LoginUserDto> loginValidator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _jwtTokenGenerator = jwtTokenGenerator;
        _loginValidator = loginValidator;
    }

    #region Validations
    public bool IsEmailExists(string email)
    {
        var isExists = _unitOfWork
            .Users
            .WhereQueryable(x => x.Email == email)
            .Any();

        return isExists;
    }
    #endregion
    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var failures = new List<ValidationFailure>
        {
            new ("Email", "This email already exists!")
        };

        if (IsEmailExists(request.Email.ToLower()))
            throw new ValidationException(failures);

        User user = new()
        {
            Id = Guid.NewGuid(),
            Fullname = request.Fullname,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveAsync();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Fullname = user.Fullname,
            Role = ((UserRolesEnum)user.Role).GetDescription()
        };
    }

    public async Task<UserDto> RegisterUserAsync(CreateUserDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors.ToList());

        if (IsEmailExists(request.Email.ToLower()))
            throw new ValidationException("Email has already taken!");

        User user = new()
        {
            Fullname = request.Fullname,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = (byte)UserRolesEnum.User
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveAsync();

        return new UserDto
        {
            Id = user.Id,
            Fullname = user.Fullname,
            Email = user.Email.ToLower(),
            Role = ((UserRolesEnum)user.Role).GetDescription()
        };
    }

    public async Task<AuthResultDto> LoginAsync(LoginUserDto request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors.ToList());

        var user = _unitOfWork
            .Users
            .WhereQueryable(x => x.Email.ToLower() == request.Email.ToLower())
            .FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new ValidationException("Invalid email or password");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResultDto
        {
            Token = token,
            Fullname = user.Fullname,
            Email = user.Email,
            Role = ((UserRolesEnum)user.Role).GetDescription()
        };
    }
}
