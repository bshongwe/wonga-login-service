using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using WongaLoginService.DTOs;
using WongaLoginService.Tests.Fixtures;

namespace WongaLoginService.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for UserController testing JWT authentication and protected endpoints.
/// </summary>
[Collection("IntegrationTests")]
public class UserControllerTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UserControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDetails_WithoutToken_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/user/details");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDetails_WithInvalidToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "invalid.token.here");

        // Act
        var response = await _client.GetAsync("/api/user/details");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDetails_WithValidToken_Returns200AndUserDetails()
    {
        // Arrange
        var password = "ValidPass123!";
        var registerDto = new RegisterRequest(
            Username: $"validuser_{Guid.NewGuid():N}",
            Email: $"valid_{Guid.NewGuid():N}@test.com",
            Password: password
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authData = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authData!.Token);

        // Act
        var response = await _client.GetAsync("/api/user/details");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().NotBeNull();
        content!.Username.Should().Be(registerDto.Username);
        content.Email.Should().Be(registerDto.Email.ToLowerInvariant());
    }

    [Fact]
    public async Task GetDetails_DoesNotExposeSensitiveData()
    {
        // Arrange
        var registerDto = new RegisterRequest(
            Username: $"sensitive_{Guid.NewGuid():N}",
            Email: $"sensitive_{Guid.NewGuid():N}@test.com",
            Password: "SensitivePass123!"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authData = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authData!.Token);

        // Act
        var response = await _client.GetAsync("/api/user/details");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        var contentString = await response.Content.ReadAsStringAsync();
        
        contentString.Should().NotContain("PasswordHash");
        contentString.Should().NotContain("$2a$");
        contentString.Should().NotContain("$2b$");
        contentString.Should().NotContain("$2y$");
    }
}
