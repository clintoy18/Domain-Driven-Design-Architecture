namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed record AuthResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        string RefreshToken,
        DateTime RefreshTokenExpiresAtUtc,
        UserResponse User);
}
