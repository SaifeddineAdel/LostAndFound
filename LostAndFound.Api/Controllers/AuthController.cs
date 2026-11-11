using FluentValidation;
using LostAndFound.Application.Features.Auth.Login.DTOs;
using LostAndFound.Application.Features.Auth.Login.Interfaces;
using LostAndFound.Application.Features.Auth.Register.DTOs;
using LostAndFound.Application.Features.Auth.Register.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRegisterService _registerService;
    private readonly ILoginService _loginService;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthController(
        IRegisterService registerService,
        ILoginService loginService,
        IValidator<RegisterRequest> registerValidator)
    {
        _registerService = registerService;
        _loginService = loginService;
        _registerValidator = registerValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var validationResult =
            await _registerValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response =
            await _registerService.RegisterAsync(request);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var response =
            await _loginService.LoginAsync(request);

        return Ok(response);
    }
}