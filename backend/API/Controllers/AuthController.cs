using Microsoft.AspNetCore.Mvc;
using backend.Core.Entities;
using backend.Core.DTOs;
using backend.Infrastructure.Persistence;
using backend.Application.Services;
using MongoDB.Driver;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;

    public AuthController(MongoDbContext context, TokenService tokenService, EmailService emailService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        if (await _context.Users.Find(u => u.Email == dto.Email).AnyAsync())
        {
            return BadRequest("Email is already registered");
        }

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsVerified = false
        };

        await _context.Users.InsertOneAsync(user);

        // Generate Registration PIN
        var pin = new Random().Next(100000, 999999).ToString();
        var regPin = new RegistrationPin
        {
            Email = user.Email,
            Pin = pin,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
        await _context.RegistrationPins.InsertOneAsync(regPin);

        // Send PIN email asynchronously
        _ = _emailService.SendRegistrationPinEmailAsync(user.Email, pin);

        return Ok(new { message = "Registration successful. Please check your email for the verification PIN." });
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<AuthResponseDto>> VerifyEmail(VerifyEmailDto dto)
    {
        var pinRecord = await _context.RegistrationPins
            .Find(p => p.Email == dto.Email && p.Pin == dto.Pin)
            .FirstOrDefaultAsync();

        if (pinRecord == null || pinRecord.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired PIN.");
        }

        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user == null) return NotFound("User not found.");

        user.IsVerified = true;
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
        await _context.RegistrationPins.DeleteManyAsync(p => p.Email == dto.Email); // clean up pins

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id!, user.Email, user.IsVerified);

        return Ok(new AuthResponseDto(userDto, token));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials");
        }

        if (!user.IsVerified)
        {
            return Unauthorized("Please verify your email before logging in.");
        }

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id!, user.Email, user.IsVerified);

        return Ok(new AuthResponseDto(userDto, token));
    }

    [HttpPost("request-password-reset")]
    public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetDto dto)
    {
        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user == null)
        {
            // Return Ok anyway to prevent email enumeration
            return Ok(new { message = "If the email is registered, a password reset PIN has been sent." });
        }

        var pin = new Random().Next(100000, 999999).ToString();
        var resetPin = new ResetPasswordPin
        {
            Email = user.Email,
            Pin = pin,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
        await _context.ResetPasswordPins.InsertOneAsync(resetPin);

        _ = _emailService.SendResetPasswordPinEmailAsync(user.Email, pin);

        return Ok(new { message = "If the email is registered, a password reset PIN has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var pinRecord = await _context.ResetPasswordPins
            .Find(p => p.Email == dto.Email && p.Pin == dto.Pin)
            .FirstOrDefaultAsync();

        if (pinRecord == null || pinRecord.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired PIN.");
        }

        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user == null) return NotFound("User not found.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
        
        await _context.ResetPasswordPins.DeleteManyAsync(p => p.Email == dto.Email); // clean up pins

        return Ok(new { message = "Password has been successfully reset. You can now login." });
    }
}
