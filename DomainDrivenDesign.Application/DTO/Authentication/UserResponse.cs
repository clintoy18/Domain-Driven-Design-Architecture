namespace DomainDrivenDesign.Application.DTO.Authentication
{
    public sealed class UserResponse
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public IReadOnlyCollection<string> Roles { get; init; } = [];
    }
}
