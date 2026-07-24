namespace DomainDrivenDesign.Application.Authentication
{
    public interface IRefreshTokenService
    {
        string GenerateToken();
        string HashToken(string token);
    }
}
