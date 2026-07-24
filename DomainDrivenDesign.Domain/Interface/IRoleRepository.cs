using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Interface
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    }
}
