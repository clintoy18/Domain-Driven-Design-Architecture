using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Interface
{
    public interface IUserRepository
    {
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
