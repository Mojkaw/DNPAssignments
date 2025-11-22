using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions;

    public HttpPostService(HttpClient client, JsonSerializerOptions jsonOptions)
    {
        this.client = client;
        this.jsonOptions = jsonOptions;
    }

    public async Task<PostDto> CreateAsync(CreatePostDto request)
    {
        var response = await client.PostAsJsonAsync("posts", request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception(content);
        return JsonSerializer.Deserialize<PostDto>(content, jsonOptions)!;
    }

    public async Task<IReadOnlyList<PostDto>> GetAllAsync()
        => await client.GetFromJsonAsync<IReadOnlyList<PostDto>>("posts", jsonOptions)
           ?? Array.Empty<PostDto>();

    public async Task<PostDto?> GetByIdAsync(int id)
    {
        var result = await client.GetFromJsonAsync<PostDto>($"posts/{id}", jsonOptions);
        if (result is null)
            throw new Exception("Post not found");
        return result;
    }

}