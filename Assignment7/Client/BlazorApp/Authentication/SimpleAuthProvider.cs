using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using ApiContracts;

namespace BlazorApp.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private ClaimsPrincipal? currentClaimsPrincipal;

    public SimpleAuthProvider(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        currentClaimsPrincipal = null;
    }

    public async Task Login(string userName, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest { Username = userName, Password = password });

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userDto.Username),
            new("Id", userDto.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "apiauth");
        currentClaimsPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentClaimsPrincipal))
        );
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = currentClaimsPrincipal ?? new ClaimsPrincipal(new ClaimsIdentity());
        return Task.FromResult(new AuthenticationState(principal));
    }

    public void Logout()
    {
        currentClaimsPrincipal = null;
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
        );
    }
}