using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Interface
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
