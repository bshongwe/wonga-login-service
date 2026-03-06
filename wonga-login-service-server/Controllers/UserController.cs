using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WongaLoginService.DTOs;
using WongaLoginService.Services;

namespace WongaLoginService.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<UserController> _logger;

    public UserController(IAuthService authService, ILogger<UserController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user details
    /// </summary>
    /// <remarks>
    /// Protected endpoint, authentication via httpOnly cookie.
    /// After logging in via /api/auth/login, cookie sent with requests.
    /// </remarks>
    [HttpGet("details")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetails()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID claim");
                return Unauthorized(new { message = "Invalid authentication token." });
            }

            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            // Catch-all for unexpected errors - prevent stack trace exposure
            _logger.LogError(ex, "Unexpected error retrieving user details");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}
