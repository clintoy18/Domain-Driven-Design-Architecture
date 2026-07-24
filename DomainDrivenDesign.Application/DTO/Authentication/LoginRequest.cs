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
        public string? Password { get; set; }
    }
}
