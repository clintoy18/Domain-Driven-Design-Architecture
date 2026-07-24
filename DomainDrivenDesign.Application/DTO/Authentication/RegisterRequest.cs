using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string? Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string? Password { get; set; }

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string? DisplayName { get; set; }
    }
}
