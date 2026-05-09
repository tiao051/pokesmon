using backend.Core.DTOs;
using backend.Core.Entities;
using backend.Infrastructure.Persistence;
using MongoDB.Driver;

namespace backend.Application.Services;

public class AuthService
{
    private readonly MongoDbContext _context;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;
    private readonly PinService _pinService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        MongoDbContext context,
        TokenService tokenService,
        EmailService emailService,
        PinService pinService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _pinService = pinService;
        _logger = logger;
    }

    public async Task<AuthOperationResult> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _context.Users.Find(u => u.Email == dto.Email).AnyAsync(ct))
            return AuthOperationResult.Conflict("Email is already registered");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsVerified = false
        };
        await _context.Users.InsertOneAsync(user, cancellationToken: ct);

        var issued = await _pinService.IssueAsync(_context.RegistrationPins, dto.Email, ct);
        await TrySendEmailAsync(() => _emailService.SendRegistrationPinEmailAsync(dto.Email, issued.PinHash), dto.Email);

        return AuthOperationResult.Success();
    }

    public async Task<AuthOperationResult<AuthResponseDto>> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken ct = default)
    {
        var verify = await _pinService.VerifyAsync(_context.RegistrationPins, dto.Email, dto.Pin, ct);
        if (verify != OtpVerifyResult.Ok) return AuthOperationResult<AuthResponseDto>.BadRequest(MapPinError(verify));

        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync(ct);
        if (user is null) return AuthOperationResult<AuthResponseDto>.NotFound("User not found");

        user.IsVerified = true;
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
        await _pinService.ConsumeAsync(_context.RegistrationPins, dto.Email, ct);

        return AuthOperationResult<AuthResponseDto>.Success(BuildAuthResponse(user));
    }

    public async Task<AuthOperationResult<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync(ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return AuthOperationResult<AuthResponseDto>.Unauthorized("Invalid credentials");

        if (!user.IsVerified)
            return AuthOperationResult<AuthResponseDto>.Unauthorized("Please verify your email before logging in");

        return AuthOperationResult<AuthResponseDto>.Success(BuildAuthResponse(user));
    }

    public async Task RequestPasswordResetAsync(RequestPasswordResetDto dto, CancellationToken ct = default)
    {
        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync(ct);
        if (user is null) return;

        var issued = await _pinService.IssueAsync(_context.ResetPasswordPins, dto.Email, ct);
        await TrySendEmailAsync(() => _emailService.SendResetPasswordPinEmailAsync(dto.Email, issued.PinHash), dto.Email);
    }

    public async Task<AuthOperationResult> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default)
    {
        var verify = await _pinService.VerifyAsync(_context.ResetPasswordPins, dto.Email, dto.Pin, ct);
        if (verify != OtpVerifyResult.Ok) return AuthOperationResult.BadRequest(MapPinError(verify));

        var user = await _context.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync(ct);
        if (user is null) return AuthOperationResult.NotFound("User not found");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
        await _pinService.ConsumeAsync(_context.ResetPasswordPins, dto.Email, ct);

        return AuthOperationResult.Success();
    }

    public async Task<AuthOperationResult> ChangePasswordAsync(string email, ChangePasswordDto dto, CancellationToken ct = default)
    {
        var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync(ct);
        if (user is null) return AuthOperationResult.NotFound("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return AuthOperationResult.BadRequest("Current password is incorrect");

        if (dto.CurrentPassword == dto.NewPassword)
            return AuthOperationResult.BadRequest("New password must differ from current");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);

        return AuthOperationResult.Success();
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var token = _tokenService.GenerateToken(user);
        return new AuthResponseDto(new UserDto(user.Id!, user.Email, user.IsVerified), token);
    }

    private async Task TrySendEmailAsync(Func<Task> send, string email)
    {
        try { await send(); }
        catch (Exception ex) { _logger.LogError(ex, "Failed sending email to {Email}", email); }
    }

    private static string MapPinError(OtpVerifyResult r) => r switch
    {
        OtpVerifyResult.NotFound => "PIN not found. Please request a new one.",
        OtpVerifyResult.Expired => "PIN has expired. Please request a new one.",
        OtpVerifyResult.TooManyAttempts => "Too many incorrect attempts. Please request a new PIN.",
        OtpVerifyResult.Invalid => "Invalid PIN.",
        _ => "Invalid request"
    };
}

public enum AuthResultKind { Ok, BadRequest, Unauthorized, NotFound, Conflict }

public class AuthOperationResult
{
    public AuthResultKind Kind { get; init; }
    public string? Error { get; init; }

    public static AuthOperationResult Success() => new() { Kind = AuthResultKind.Ok };
    public static AuthOperationResult BadRequest(string error) => new() { Kind = AuthResultKind.BadRequest, Error = error };
    public static AuthOperationResult Unauthorized(string error) => new() { Kind = AuthResultKind.Unauthorized, Error = error };
    public static AuthOperationResult NotFound(string error) => new() { Kind = AuthResultKind.NotFound, Error = error };
    public static AuthOperationResult Conflict(string error) => new() { Kind = AuthResultKind.Conflict, Error = error };
}

public class AuthOperationResult<T>
{
    public AuthResultKind Kind { get; init; }
    public string? Error { get; init; }
    public T? Value { get; init; }

    public static AuthOperationResult<T> Success(T value) => new() { Kind = AuthResultKind.Ok, Value = value };
    public static AuthOperationResult<T> BadRequest(string error) => new() { Kind = AuthResultKind.BadRequest, Error = error };
    public static AuthOperationResult<T> Unauthorized(string error) => new() { Kind = AuthResultKind.Unauthorized, Error = error };
    public static AuthOperationResult<T> NotFound(string error) => new() { Kind = AuthResultKind.NotFound, Error = error };
}
