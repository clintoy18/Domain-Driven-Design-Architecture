using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Interface;
using DomainDrivenDesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesign.Infrastructure.Repositories
{
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(role => role.NormalizedName == normalizedName, cancellationToken);
        }
    }
}
