using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions;

    public HttpCommentService(HttpClient client, JsonSerializerOptions jsonOptions)
    {
        this.client = client;
        this.jsonOptions = jsonOptions;
    }

    public async Task<CommentDto> AddAsync(CreateCommentDto request)
    {
        var response = await client.PostAsJsonAsync("comments", request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception(content);
        return JsonSerializer.Deserialize<CommentDto>(content, jsonOptions)!;
    }

    public async Task<IReadOnlyList<CommentDto>> GetByPostIdAsync(int postId)
        => await client.GetFromJsonAsync<IReadOnlyList<CommentDto>>($"comments?postId={postId}", jsonOptions)
           ?? Array.Empty<CommentDto>();
}