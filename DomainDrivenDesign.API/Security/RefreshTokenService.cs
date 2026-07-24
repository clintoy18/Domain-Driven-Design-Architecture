using System.Security.Cryptography;
using System.Text;
using DomainDrivenDesign.Application.Authentication;

namespace DomainDrivenDesign.API.Security
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        public string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string HashToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(tokenBytes);

            return Convert.ToBase64String(hash);
        }
    }
}
