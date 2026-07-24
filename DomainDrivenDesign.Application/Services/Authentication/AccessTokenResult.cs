namespace DomainDrivenDesign.Application.Services.Authentication
{
    public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
}
