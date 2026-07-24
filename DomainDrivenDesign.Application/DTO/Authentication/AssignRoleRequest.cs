using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed record AssignRoleRequest(
        [property: Required]
        [property: RegularExpression("^(Admin|Author|Reader)$")]
        string? RoleName);
}
