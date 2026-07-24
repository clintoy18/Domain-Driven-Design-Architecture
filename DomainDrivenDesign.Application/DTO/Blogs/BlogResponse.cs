namespace DomainDrivenDesign.Application.DTO.Blogs
{
    public sealed record BlogResponse(
        int Id,
        string Name,
        string Description,
        string Author,
        string ImageUrl);
}
