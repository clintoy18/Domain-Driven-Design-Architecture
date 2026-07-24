namespace DomainDrivenDesign.Application.Services.Authentication
{
    public interface IRefreshTokenService
    {
        string GenerateToken();
        string HashToken(string token);
    }
}
