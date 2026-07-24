using DomainDrivenDesign.Application.DTO.Authentication;

namespace DomainDrivenDesign.Application.Authentication
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(RevokeRefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserResponse> AssignRoleAsync(Guid userId, AssignRoleRequest request, CancellationToken cancellationToken = default);
    }
}
