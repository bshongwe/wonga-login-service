using System.ComponentModel.DataAnnotations;

namespace WongaLoginService.DTOs;

public record LoginRequest(
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password
);

public record RegisterRequest(
    [Required, MinLength(3), MaxLength(20), RegularExpression(@"^\w+$")] string Username,
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password
);

public record AuthResponse(string Token, UserResponse User);

public record UserResponse(
    string Id,
    string Username,
    string Email,
    string CreatedAt
);
