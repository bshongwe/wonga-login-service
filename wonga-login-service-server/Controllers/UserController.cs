using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WongaLoginService.Services;

namespace WongaLoginService.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;

    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("details")]
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
