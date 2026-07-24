using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed class RefreshTokenRequest
    {
        [Required]
        [StringLength(512, MinimumLength = 32)]
        public string? RefreshToken { get; set; }
    }
}
