using System.ComponentModel.DataAnnotations;

namespace backend.Core.DTOs;

public record UserDto(string Id, string Email, bool IsVerified);

public record AuthResponseDto(UserDto User, string Token);

public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record VerifyEmailDto(
    [Required, EmailAddress] string Email,
    [Required, RegularExpression(@"^\d{6}$")] string Pin
);

public record RequestPasswordResetDto(
    [Required, EmailAddress] string Email
);

public record ResetPasswordDto(
    [Required, EmailAddress] string Email,
    [Required, RegularExpression(@"^\d{6}$")] string Pin,
    [Required, MinLength(8)] string NewPassword
);

public record ChangePasswordDto(
    [Required] string CurrentPassword,
    [Required, MinLength(8)] string NewPassword
);
