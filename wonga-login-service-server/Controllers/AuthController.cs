using Microsoft.AspNetCore.Mvc;
using WongaLoginService.DTOs;
using WongaLoginService.Services;

namespace WongaLoginService.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <remarks>
    /// Creates new user account, returns JWT token response.
    /// Token set as httpOnly cookie ('auth_token').
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            // Hide detailed ModelState errors
            // CWE-209 fix
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(new { message = "Invalid input data.", errors });
        }

        try
        {
            var response = await _authService.RegisterAsync(request);
            
            // Set httpOnly cookie with SameSite for CSRF protection
            Response.Cookies.Append("auth_token", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromHours(1),
                Path = "/"
            });
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            // Log full details server-side
            _logger.LogWarning(ex, "Registration failed for request");
            // Return sanitized message to client
            return BadRequest(new { message = "Registration failed. Please check your input." });
        }
        catch (Exception ex)
        {
            // Catch-all for unexpected errors
            // CWE-209 fix
            _logger.LogError(ex, "Unexpected error during registration");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    /// <summary>
    /// Login with existing credentials
    /// </summary>
    /// <remarks>
    /// Authenticates user, returns JWT token response.
    /// Token set as httpOnly cookie ('auth_token').
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            // Hide detailed ModelState errors
            // CWE-209 fix
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(new { message = "Invalid input data.", errors });
        }

        try
        {
            var response = await _authService.LoginAsync(request);
            
            // Set httpOnly cookie with SameSite for CSRF protection
            Response.Cookies.Append("auth_token", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromHours(1),
                Path = "/"
            });
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Log full details server-side
            _logger.LogWarning(ex, "Login failed for request");
            // Return generic message to prevent user enumeration
            return Unauthorized(new { message = "Invalid credentials." });
        }
        catch (Exception ex)
        {
            // Catch-all for unexpected errors
            // CWE-209 fix
            _logger.LogError(ex, "Unexpected error during login");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}
