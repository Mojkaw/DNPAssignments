using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions;

    public HttpUserService(HttpClient client, JsonSerializerOptions jsonOptions)
    {
        this.client = client;
        this.jsonOptions = jsonOptions;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        var response = await client.PostAsJsonAsync("users", request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<UserDto>(content, jsonOptions)!;
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        var response = await client.PutAsJsonAsync($"users/{id}", request);
        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
        => await client.GetFromJsonAsync<IReadOnlyList<UserDto>>("users", jsonOptions)
           ?? Array.Empty<UserDto>();

    public async Task<UserDto> GetByIdAsync(int id)
        => await client.GetFromJsonAsync<UserDto>($"users/{id}", jsonOptions)
           ?? throw new Exception("User not found");
}