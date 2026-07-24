using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed class AssignRoleRequest
    {
        [Required]
        [RegularExpression("^(Admin|Author|Reader)$")]
        public string? RoleName { get; set; }
    }
}
