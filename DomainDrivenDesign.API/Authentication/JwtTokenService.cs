using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainDrivenDesign.Application.Authentication;
using DomainDrivenDesign.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DomainDrivenDesign.API.Authentication
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public AccessTokenResult CreateToken(User user, IReadOnlyCollection<string> roles)
        {
            var now = DateTime.UtcNow;
            var expiresAtUtc = now.AddMinutes(_options.AccessTokenMinutes);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.DisplayName)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new AccessTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }
    }
}
