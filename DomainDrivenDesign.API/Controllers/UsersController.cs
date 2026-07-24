using DomainDrivenDesign.Application.Authentication;
using DomainDrivenDesign.Application.DTO.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("{id:guid}/roles")]
        public async Task<IActionResult> AssignRole(Guid id, AssignRoleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _authService.AssignRoleAsync(id, request, cancellationToken));
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}
