using ApiContracts;

namespace BlazorApp.Services;

public interface ICommentService
{
    Task<CommentDto> AddAsync(CreateCommentDto request);
    Task<IReadOnlyList<CommentDto>> GetByPostIdAsync(int postId);
}