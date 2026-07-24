using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Application.DTO.Blogs
{
    public sealed class UpdateBlogRequest
    {
        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1_000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [Url]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
