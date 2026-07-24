using DomainDrivenDesign.Application.DTO.Blogs;
using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Interface;

namespace DomainDrivenDesign.Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<BlogResponse> CreateAsync(CreateBlogRequest request)
        {
            var blog = new Blog
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Author = request.Author.Trim(),
                ImageUrl = request.ImageUrl.Trim()
            };

            var createdBlog = await _blogRepository.CreateAsync(blog);
            return ToResponse(createdBlog);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _blogRepository.DeleteAsync(id) > 0;
        }

        public async Task<List<BlogResponse>> GetAllAsync()
        {
            var blogs = await _blogRepository.GetAllAsync();
            return blogs.Select(ToResponse).ToList();
        }

        public async Task<BlogResponse?> GetByIdAsync(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            return blog is null ? null : ToResponse(blog);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBlogRequest request)
        {
            var blog = new Blog
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Author = request.Author.Trim(),
                ImageUrl = request.ImageUrl.Trim()
            };

            return await _blogRepository.UpdateAsync(id, blog) > 0;
        }

        private static BlogResponse ToResponse(Blog blog)
        {
            return new BlogResponse(
                blog.Id,
                blog.Name,
                blog.Description,
                blog.Author,
                blog.ImageUrl);
        }
    }
}
