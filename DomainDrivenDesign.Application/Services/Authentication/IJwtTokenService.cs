using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Application.Services.Authentication
{
    public interface IJwtTokenService
    {
        AccessTokenResult CreateToken(User user, IReadOnlyCollection<string> roles);
    }
}
