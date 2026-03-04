using Microsoft.EntityFrameworkCore;
using WongaLoginService.Data;
using WongaLoginService.DTOs;
using WongaLoginService.Models;

namespace WongaLoginService.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse?> GetUserByIdAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check existence with AsNoTracking for better performance
        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already exists");

        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Username == request.Username))
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse(token, MapToUserResponse(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Use AsNoTracking for read-only query
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse(token, MapToUserResponse(user));
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
    {
        // Use AsNoTracking and FindAsync is not compatible, use FirstOrDefaultAsync
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        return user == null ? null : MapToUserResponse(user);
    }

    private static UserResponse MapToUserResponse(User user) =>
        new(user.Id.ToString(), user.Username, user.Email, user.CreatedAt.ToString("o"));
}
