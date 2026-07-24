namespace DomainDrivenDesign.Application.Authentication
{
    public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
}
