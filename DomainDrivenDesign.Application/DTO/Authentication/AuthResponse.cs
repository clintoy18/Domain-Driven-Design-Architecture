namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed class AuthResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; init; }
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime RefreshTokenExpiresAtUtc { get; init; }
        public UserResponse User { get; init; } = new();
    }
}
