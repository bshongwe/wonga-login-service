using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WongaLoginService.DTOs;
using WongaLoginService.Tests.Fixtures;

namespace WongaLoginService.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for AuthController using PostgreSQL via Testcontainers.
/// Tests full HTTP request/response cycle for DB operations.
/// </summary>
[Collection("IntegrationTests")]
public class AuthControllerTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns200AndToken()
    {
        // Arrange
        var registerDto = new RegisterRequest(
            Username: $"user_{Guid.NewGuid():N}",
            Email: $"user_{Guid.NewGuid():N}@test.com",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content!.User.Username.Should().Be(registerDto.Username);
        content.User.Email.Should().Be(registerDto.Email.ToLowerInvariant());
        content.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        // Arrange
        var email = $"duplicate_{Guid.NewGuid():N}@test.com";
        var firstUser = new RegisterRequest(
            Username: "firstuser",
            Email: email,
            Password: "Pass123!"
        );
        await _client.PostAsJsonAsync("/api/auth/register", firstUser);

        var duplicateUser = new RegisterRequest(
            Username: "seconduser",
            Email: email,
            Password: "Pass456!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateUser);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        content.Should().NotContain("Exception");
        content.Should().NotContain("StackTrace");
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200AndToken()
    {
        // Arrange
        var password = "LoginPass123!";
        var registerDto = new RegisterRequest(
            Username: $"loginuser_{Guid.NewGuid():N}",
            Email: $"login_{Guid.NewGuid():N}@test.com",
            Password: password
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginRequest(
            Email: registerDto.Email,
            Password: password
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content!.User.Email.Should().Be(registerDto.Email.ToLowerInvariant());
        content.Token.Should().NotBeNullOrEmpty();
        
        var tokenParts = content.Token.Split('.');
        tokenParts.Length.Should().Be(3);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_Returns401()
    {
        // Arrange
        var loginDto = new LoginRequest(
            Email: "nonexistent@test.com",
            Password: "Pass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotContain("Exception");
        content.Should().NotContain("at ");
    }

    [Fact]
    public async Task Register_StoresPasswordAsHash_NotPlainText()
    {
        // Arrange
        var password = "SecretPass123!";
        var registerDto = new RegisterRequest(
            Username: $"hashtest_{Guid.NewGuid():N}",
            Email: $"hashtest_{Guid.NewGuid():N}@test.com",
            Password: password
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        using var dbContext = _factory.GetDbContext();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email.ToLowerInvariant());
        
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBe(password);
        user.PasswordHash.Should().StartWith("$2");
    }
}
