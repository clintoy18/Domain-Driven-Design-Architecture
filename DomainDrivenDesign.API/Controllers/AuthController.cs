using System.Security.Claims;
using DomainDrivenDesign.Application.DTO.Authentication;
using DomainDrivenDesign.Application.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _authService.RegisterAsync(request, cancellationToken));
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _authService.LoginAsync(request, cancellationToken));
            }
            catch (UnauthorizedAccessException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _authService.RefreshAsync(request, cancellationToken));
            }
            catch (UnauthorizedAccessException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(RevokeRefreshTokenRequest request, CancellationToken cancellationToken)
        {
            await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            return Ok(await _authService.GetCurrentUserAsync(userId, cancellationToken));
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User id claim is missing or invalid.");
            }

            return userId;
        }
    }
}
