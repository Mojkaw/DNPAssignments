using ApiContracts;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<PostDto> CreateAsync(CreatePostDto request);
    Task<PostDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<PostDto>> GetAllAsync();
}