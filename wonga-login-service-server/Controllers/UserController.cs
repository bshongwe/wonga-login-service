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

    public UserController(IAuthService authService)
    {
        _authService = authService;
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _authService.GetUserByIdAsync(userId);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }
}
