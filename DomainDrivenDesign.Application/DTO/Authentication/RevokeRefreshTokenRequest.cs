using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed record RevokeRefreshTokenRequest(
        [property: Required]
        [property: StringLength(512, MinimumLength = 32)]
        string? RefreshToken);
}
