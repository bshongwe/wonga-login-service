namespace WongaLoginService.DTOs;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(
    string Username,
    string Email,
    string Password
);

public record AuthResponse(string Token, UserResponse User);

public record UserResponse(
    string Id,
    string Username,
    string Email,
    string CreatedAt
);
