using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed record LoginRequest(
        [property: Required]
        [property: EmailAddress]
        [property: StringLength(320)]
        string? Email,

        [property: Required]
        [property: StringLength(128, MinimumLength = 8)]
        string? Password);
}
