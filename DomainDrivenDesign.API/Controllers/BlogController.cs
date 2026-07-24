using DomainDrivenDesign.Application.DTO.Blogs;
using DomainDrivenDesign.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "CanReadBlogs")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            return blog is null ? NotFound() : Ok(blog);
        }

        [HttpPost]
        [Authorize(Policy = "CanManageBlogs")]
        public async Task<IActionResult> CreateBlog(CreateBlogRequest request)
        {
            var createdBlog = await _blogService.CreateAsync(request);
            return CreatedAtAction(nameof(GetBlogById), new { id = createdBlog.Id }, createdBlog);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanManageBlogs")]
        public async Task<IActionResult> UpdateBlog(int id, UpdateBlogRequest request)
        {
            var wasUpdated = await _blogService.UpdateAsync(id, request);
            return wasUpdated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var wasDeleted = await _blogService.DeleteAsync(id);
            return wasDeleted ? NoContent() : NotFound();
        }
    }
}
