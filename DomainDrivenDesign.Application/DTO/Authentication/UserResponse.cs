namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed record UserResponse(Guid Id, string Email, string DisplayName, IReadOnlyCollection<string> Roles);
}
