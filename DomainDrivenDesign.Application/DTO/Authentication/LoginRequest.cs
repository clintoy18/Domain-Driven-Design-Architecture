using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string? Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string? Password { get; set; }
    }
}
