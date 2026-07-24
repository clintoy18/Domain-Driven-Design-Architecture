using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainDrivenDesign.Application.Services;

namespace DomainDrivenDesign.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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



    }
}
