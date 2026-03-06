using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WongaLoginService.Data;
using WongaLoginService.DTOs;
using WongaLoginService.Models;
using WongaLoginService.Services;

namespace WongaLoginService.Tests.Unit.Services;

/// <summary>
/// Unit tests for AuthService using in-memory database and mocked dependencies.
/// Tests core authentication logic in isolation.
/// </summary>
public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Setup in-memory database for unit testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _context = new AppDbContext(options);

        // Setup mock JWT service
        _mockJwtService = new Mock<IJwtService>();
        _mockJwtService.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("test-jwt-token-123");

        // Create the service under test
        _authService = new AuthService(_context, _mockJwtService.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserSuccessfully()
    {
        // Arrange
        var registerDto = new RegisterRequest(
            Username: "testuser",
            Email: "test@test.com",
            Password: "SecurePass123!" // ggignore: test fixture password
        );

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.User.Username.Should().Be("testuser");
        result.User.Email.Should().Be("test@test.com");
        result.Token.Should().NotBeNullOrEmpty();

        // Verify user was saved to database
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        savedUser.Should().NotBeNull();
        savedUser!.Username.Should().Be("testuser");
        savedUser.PasswordHash.Should().NotBeNullOrEmpty();
        savedUser.PasswordHash.Should().NotBe("SecurePass123!"); // Should be hashed
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "existinguser",
            Email = "duplicate@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerDto = new RegisterRequest(
            Username: "newuser",
            Email: "duplicate@test.com", // Same email
            Password: "NewPass123!"
        );

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*failed*");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "duplicateusername",
            Email = "user1@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerDto = new RegisterRequest(
            Username: "duplicateusername", // Same username
            Email: "user2@test.com",
            Password: "NewPass123!"
        );

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*failed*");
    }

    [Fact]
    public async Task RegisterAsync_HashesPasswordCorrectly()
    {
        // Arrange
        var registerDto = new RegisterRequest(
            Username: "hashtest",
            Email: "hash@test.com",
            Password: "PlainTextPassword123!"
        );

        // Act
        await _authService.RegisterAsync(registerDto);

        // Assert
        var user = await _context.Users.FirstAsync(u => u.Email == "hash@test.com");
        user.PasswordHash.Should().NotBe("PlainTextPassword123!");
        
        // Verify BCrypt hash format (starts with $2a$, $2b$, or $2y$)
        user.PasswordHash.Should().MatchRegex(@"^\$2[aby]\$\d+\$");
        
        // Verify password can be verified
        BCrypt.Net.BCrypt.Verify("PlainTextPassword123!", user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var password = "LoginPass123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "loginuser",
            Email = "login@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginRequest(
            Email: "login@test.com",
            Password: password
        );

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.User.Username.Should().Be("loginuser");
        result.User.Email.Should().Be("login@test.com");
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginRequest(
            Email: "nonexistent@test.com",
            Password: "SomePass123!"
        );

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid credentials*");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginRequest(
            Email: "user@test.com",
            Password: "WrongPassword123!" // Incorrect password
        );

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid credentials*");
    }

    [Fact]
    public async Task RegisterAsync_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var beforeRegistration = DateTime.UtcNow;
        var registerDto = new RegisterRequest(
            Username: "timestamptest",
            Email: "timestamp@test.com",
            Password: "Pass123!"
        );

        // Act
        await _authService.RegisterAsync(registerDto);
        var afterRegistration = DateTime.UtcNow;

        // Assert
        var user = await _context.Users.FirstAsync(u => u.Email == "timestamp@test.com");
        user.CreatedAt.Should().BeOnOrAfter(beforeRegistration);
        user.CreatedAt.Should().BeOnOrBefore(afterRegistration);
        user.UpdatedAt.Should().BeOnOrAfter(beforeRegistration);
        user.UpdatedAt.Should().BeOnOrBefore(afterRegistration);
        user.CreatedAt.Should().BeCloseTo(user.UpdatedAt, TimeSpan.FromMilliseconds(1)); // Should be equal on creation
    }

    [Fact]
    public async Task LoginAsync_GeneratesValidJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "jwttest",
            Email = "jwt@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("JwtPass123!"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginRequest(
            Email: "jwt@test.com",
            Password: "JwtPass123!"
        );

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Token.Should().NotBeNullOrEmpty();
        result.Token.Should().Be("test-jwt-token-123"); // From mock
    }
}
