using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WongaLoginService.DTOs;
using WongaLoginService.Tests.Fixtures;

namespace WongaLoginService.Tests.Integration.EndToEnd;

/// <summary>
/// End-to-end tests that verify complete authentication flows.
/// </summary>
[Collection("IntegrationTests")]
public class FullAuthFlowTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public FullAuthFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteFlow_RegisterLoginAccessProtectedEndpoint_Success()
    {
        // STEP 1: REGISTER
        var password = "E2EPass123!";
        var registerDto = new RegisterRequest(
            Username: $"e2euser_{Guid.NewGuid():N}",
            Email: $"e2e_{Guid.NewGuid():N}@test.com",
            Password: password
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        
        registerResponse.Should().HaveStatusCode(HttpStatusCode.OK);
        var registerAuth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        registerAuth.Should().NotBeNull();
        registerAuth!.Token.Should().NotBeNullOrEmpty();

        // STEP 2: LOGIN
        var loginDto = new LoginRequest(
            Email: registerDto.Email,
            Password: password
        );

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        
        loginResponse.Should().HaveStatusCode(HttpStatusCode.OK);
        var loginAuth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        loginAuth.Should().NotBeNull();
        loginAuth!.Token.Should().NotBeNullOrEmpty();

        // STEP 3: ACCESS PROTECTED ENDPOINT
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", loginAuth.Token);

        var detailsResponse = await _client.GetAsync("/api/user/details");
        
        detailsResponse.Should().HaveStatusCode(HttpStatusCode.OK);
        var userDetails = await detailsResponse.Content.ReadFromJsonAsync<UserResponse>();
        userDetails.Should().NotBeNull();
        userDetails!.Username.Should().Be(registerDto.Username);
    }

    [Fact]
    public async Task CompleteFlow_RegisterWithRegTokenAccessProtectedEndpoint_Success()
    {
        // STEP 1: REGISTER
        var registerDto = new RegisterRequest(
            Username: $"regtoken_{Guid.NewGuid():N}",
            Email: $"regtoken_{Guid.NewGuid():N}@test.com",
            Password: "RegToken123!"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var registerAuth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // STEP 2: USE REGISTER TOKEN DIRECTLY
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", registerAuth!.Token);

        var detailsResponse = await _client.GetAsync("/api/user/details");
        
        detailsResponse.Should().HaveStatusCode(HttpStatusCode.OK);
        var userDetails = await detailsResponse.Content.ReadFromJsonAsync<UserResponse>();
        userDetails!.Email.Should().Be(registerDto.Email.ToLowerInvariant());
    }

    [Fact]
    public async Task CompleteFlow_PasswordCaseSensitive()
    {
        // Register
        var correctPassword = "CorrectPass123!";
        var registerDto = new RegisterRequest(
            Username: $"casetest_{Guid.NewGuid():N}",
            Email: $"casetest_{Guid.NewGuid():N}@test.com",
            Password: correctPassword
        );

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Try with wrong case
        var wrongCaseLogin = new LoginRequest(
            Email: registerDto.Email,
            Password: "correctpass123!"
        );

        var wrongCaseResponse = await _client.PostAsJsonAsync("/api/auth/login", wrongCaseLogin);
        wrongCaseResponse.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

        // Try with correct password
        var correctLogin = new LoginRequest(
            Email: registerDto.Email,
            Password: correctPassword
        );

        var correctResponse = await _client.PostAsJsonAsync("/api/auth/login", correctLogin);
        correctResponse.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CompleteFlow_VerifyDatabasePersistence()
    {
        // Register
        var registerDto = new RegisterRequest(
            Username: $"dbtest_{Guid.NewGuid():N}",
            Email: $"dbtest_{Guid.NewGuid():N}@test.com",
            Password: "DbTest123!"
        );

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Verify in DB
        using var dbContext = _factory.GetDbContext();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email.ToLowerInvariant());

        user.Should().NotBeNull();
        user!.Username.Should().Be(registerDto.Username);
        user.Email.Should().Be(registerDto.Email.ToLowerInvariant());
        user.PasswordHash.Should().NotBeNullOrEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}
