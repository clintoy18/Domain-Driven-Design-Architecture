using DomainDrivenDesign.Application.DTO.Authentication;
using DomainDrivenDesign.Application.Services.Authentication;
using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Interface;

namespace DomainDrivenDesign.Application.Services.Authentication
{
    public sealed class AuthService : IAuthService
    {
        private const string AdminRole = "ADMIN";
        private const string ReaderRole = "READER";
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IRefreshTokenRepository _refreshTokens;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUserRepository users,
            IRoleRepository roles,
            IRefreshTokenRepository refreshTokens,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService)
        {
            _users = users;
            _roles = roles;
            _refreshTokens = refreshTokens;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var email = NormalizeEmail(request.Email);
            var rawEmail = request.Email?.Trim() ?? string.Empty;
            var displayName = request.DisplayName?.Trim() ?? string.Empty;
            var password = request.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new InvalidOperationException("Display name is required.");
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                throw new InvalidOperationException("Password must be at least 8 characters.");
            }

            if (await _users.ExistsByNormalizedEmailAsync(email, cancellationToken))
            {
                throw new InvalidOperationException("Email is already registered.");
            }

            var isFirstUser = !await _users.AnyAsync(cancellationToken);
            var defaultRoleName = isFirstUser ? AdminRole : ReaderRole;
            var defaultRole = await _roles.GetByNormalizedNameAsync(defaultRoleName, cancellationToken)
                ?? throw new InvalidOperationException($"The {defaultRoleName} role is not configured.");

            var user = new User
            {
                Email = rawEmail,
                NormalizedEmail = email,
                DisplayName = displayName,
                PasswordHash = _passwordHasher.HashPassword(password),
                CreatedAtUtc = DateTime.UtcNow
            };

            user.UserRoles.Add(new UserRole
            {
                User = user,
                Role = defaultRole,
                RoleId = defaultRole.Id
            });

            await _users.AddAsync(user, cancellationToken);
            await _users.SaveChangesAsync(cancellationToken);

            return await CreateAuthResponseAsync(user, [defaultRole.Name], cancellationToken);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var email = NormalizeEmail(request.Email);
            var user = await _users.GetByNormalizedEmailAsync(email, cancellationToken);

            if (user is null ||
                !user.IsActive ||
                string.IsNullOrWhiteSpace(request.Password) ||
                !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var roles = GetRoleNames(user);
            return await CreateAuthResponseAsync(user, roles, cancellationToken);
        }

        public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
            }

            var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
            var refreshToken = await _refreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

            if (refreshToken is null || !refreshToken.IsActive || !refreshToken.User.IsActive)
            {
                throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
            }

            refreshToken.RevokedAtUtc = DateTime.UtcNow;

            var roles = GetRoleNames(refreshToken.User);
            return await CreateAuthResponseAsync(refreshToken.User, roles, cancellationToken);
        }

        public async Task RevokeRefreshTokenAsync(RevokeRefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return;
            }

            var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
            var refreshToken = await _refreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

            if (refreshToken is null || !refreshToken.IsActive)
            {
                return;
            }

            refreshToken.RevokedAtUtc = DateTime.UtcNow;
            await _refreshTokens.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _users.GetByIdAsync(userId, cancellationToken)
                ?? throw new InvalidOperationException("User was not found.");

            return ToUserResponse(user, GetRoleNames(user));
        }

        public async Task<UserResponse> AssignRoleAsync(Guid userId, AssignRoleRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _users.GetByIdAsync(userId, cancellationToken)
                ?? throw new InvalidOperationException("User was not found.");

            var normalizedRoleName = NormalizeRoleName(request.RoleName);
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new InvalidOperationException("Role is required.");
            }

            var role = await _roles.GetByNormalizedNameAsync(normalizedRoleName, cancellationToken)
                ?? throw new InvalidOperationException("Role was not found.");

            if (user.UserRoles.All(userRole => userRole.RoleId != role.Id))
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    Role = role
                });

                user.UpdatedAtUtc = DateTime.UtcNow;
                await _users.SaveChangesAsync(cancellationToken);
            }

            return ToUserResponse(user, GetRoleNames(user));
        }

        private async Task<AuthResponse> CreateAuthResponseAsync(
            User user,
            IReadOnlyCollection<string> roles,
            CancellationToken cancellationToken)
        {
            var accessToken = _jwtTokenService.CreateToken(user, roles);
            var refreshTokenValue = _refreshTokenService.GenerateToken();
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _refreshTokenService.HashToken(refreshTokenValue),
                ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime)
            };

            await _refreshTokens.AddAsync(refreshToken, cancellationToken);
            await _refreshTokens.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
                User = ToUserResponse(user, roles)
            };
        }

        private static IReadOnlyCollection<string> GetRoleNames(User user)
        {
            return user.UserRoles
                .Select(userRole => userRole.Role.Name)
                .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(roleName => roleName)
                .ToArray();
        }

        private static UserResponse ToUserResponse(User user, IReadOnlyCollection<string> roles)
        {
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = roles
            };
        }

        private static string NormalizeEmail(string? email)
        {
            return email?.Trim().ToUpperInvariant() ?? string.Empty;
        }

        private static string NormalizeRoleName(string? roleName)
        {
            return roleName?.Trim().ToUpperInvariant() ?? string.Empty;
        }
    }
}
