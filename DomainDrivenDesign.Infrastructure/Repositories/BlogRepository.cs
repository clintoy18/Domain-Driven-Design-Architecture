using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Interface;
using DomainDrivenDesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<List<Blog>> GetAllAsync()
        {
            return await _context.Blogs.ToListAsync();
        }

        public async Task<List<Blog>> GetByIdAsync(int id)
        {
            return await _context.Blogs.AsNoTracking()
                .Where(b => b.Id == id)
                .ToListAsync();
        }

        public async Task<int> UpdateAsync(int id, Blog blog)
        {
            return await _context.Blogs
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, blog.Id)
                    .SetProperty(m => m.Name, blog.Name)
                    .SetProperty(m => m.Description, blog.Description)
                    .SetProperty(m => m.Author, blog.Author)
                    .SetProperty(m => m.ImageUrl, blog.ImageUrl)
                );
        }
    }
}
