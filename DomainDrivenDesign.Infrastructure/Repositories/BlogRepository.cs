using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Interface;
using DomainDrivenDesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesign.Infrastructure.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Blog> CreateAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _context.Blogs
                .Where(blog => blog.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<List<Blog>> GetAllAsync()
        {
            return await _context.Blogs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .AsNoTracking()
                .FirstOrDefaultAsync(blog => blog.Id == id);
        }

        public async Task<int> UpdateAsync(int id, Blog blog)
        {
            return await _context.Blogs
                .Where(existingBlog => existingBlog.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(existingBlog => existingBlog.Name, blog.Name)
                    .SetProperty(existingBlog => existingBlog.Description, blog.Description)
                    .SetProperty(existingBlog => existingBlog.Author, blog.Author)
                    .SetProperty(existingBlog => existingBlog.ImageUrl, blog.ImageUrl));
        }
    }
}
