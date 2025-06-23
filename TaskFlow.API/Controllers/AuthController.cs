using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTO.User;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Domain.Enums;

namespace TaskFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AuthController : ControllerBase
{
    private readonly IUserAdderService _userAdderService;

    public AuthController(IUserAdderService userAdderService)
    {
        _userAdderService = userAdderService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto request)
    {
        var result = await _userAdderService.RegisterUserAsync(request);

        return Ok(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        var result = await _userAdderService.LoginAsync(request);

        return Ok(result);
    }

    [HttpGet("Me")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Message = "You are authorized! 👍",
            UserId = userId,
            Email = email,
            Role = role
        });
    }

    [HttpGet("MeAsAdmin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetCurrentAdmin()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Message = "You are authorized as Admin! 👍",
            UserId = userId,
            Email = email,
            Role = role
        });
    }
}
