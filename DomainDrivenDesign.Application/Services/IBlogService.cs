using DomainDrivenDesign.Application.DTO.Blogs;

namespace DomainDrivenDesign.Application.Services
{
    public interface IBlogService
    {
        Task<List<BlogResponse>> GetAllAsync();
        Task<BlogResponse?> GetByIdAsync(int id);
        Task<BlogResponse> CreateAsync(CreateBlogRequest request);
        Task<bool> UpdateAsync(int id, UpdateBlogRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
