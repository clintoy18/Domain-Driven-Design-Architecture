using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed record RegisterRequest(
        [property: Required]
        [property: EmailAddress]
        [property: StringLength(320)]
        string? Email,

        [property: Required]
        [property: StringLength(128, MinimumLength = 8)]
        string? Password,

        [property: Required]
        [property: StringLength(120, MinimumLength = 2)]
        string? DisplayName);
}
