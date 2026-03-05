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

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
            return BadRequest(ModelState);

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
            return BadRequest(new { message = ex.Message });
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
            return BadRequest(ModelState);

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
            return Unauthorized(new { message = ex.Message });
        }
    }
}
