namespace backend.Core.DTOs;

public record UserDto(string Id, string Email, bool IsVerified);
public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(UserDto User, string Token);
public record VerifyEmailDto(string Email, string Pin);
public record RequestPasswordResetDto(string Email);
public record ResetPasswordDto(string Email, string Pin, string NewPassword);
