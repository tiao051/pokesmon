using System.Security.Claims;
using backend.Application.Services;
using backend.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);
        return ToActionResult(result, _ => Ok(new { message = "Registration successful. Please check your email for the verification PIN." }));
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail(VerifyEmailDto dto, CancellationToken ct)
    {
        var result = await _authService.VerifyEmailAsync(dto, ct);
        return ToActionResult(result, v => Ok(v));
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        return ToActionResult(result, v => Ok(v));
    }

    [HttpPost("request-password-reset")]
    public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetDto dto, CancellationToken ct)
    {
        await _authService.RequestPasswordResetAsync(dto, ct);
        return Ok(new { message = "If the email is registered, a password reset PIN has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordDto dto, CancellationToken ct)
    {
        var result = await _authService.ResetPasswordAsync(dto, ct);
        return ToActionResult(result, _ => Ok(new { message = "Password has been successfully reset. You can now login." }));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken ct)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        if (string.IsNullOrEmpty(email)) return Unauthorized(new { error = "Invalid token" });

        var result = await _authService.ChangePasswordAsync(email, dto, ct);
        return ToActionResult(result, _ => Ok(new { message = "Password changed successfully." }));
    }

    private ActionResult ToActionResult(AuthOperationResult result, Func<object?, ActionResult> onSuccess)
        => result.Kind switch
        {
            AuthResultKind.Ok => onSuccess(null),
            AuthResultKind.BadRequest => BadRequest(new { error = result.Error }),
            AuthResultKind.Unauthorized => Unauthorized(new { error = result.Error }),
            AuthResultKind.NotFound => NotFound(new { error = result.Error }),
            AuthResultKind.Conflict => Conflict(new { error = result.Error }),
            _ => StatusCode(500, new { error = "Unexpected error" })
        };

    private ActionResult ToActionResult<T>(AuthOperationResult<T> result, Func<T, ActionResult> onSuccess)
        => result.Kind switch
        {
            AuthResultKind.Ok => onSuccess(result.Value!),
            AuthResultKind.BadRequest => BadRequest(new { error = result.Error }),
            AuthResultKind.Unauthorized => Unauthorized(new { error = result.Error }),
            AuthResultKind.NotFound => NotFound(new { error = result.Error }),
            AuthResultKind.Conflict => Conflict(new { error = result.Error }),
            _ => StatusCode(500, new { error = "Unexpected error" })
        };
}
