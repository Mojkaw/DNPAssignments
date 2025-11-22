using BlazorApp.Components;
using BlazorApp.Services;
using BlazorApp.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure HTTP client for API
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5101/");
});

// Make DI inject the same HttpClient everywhere
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

// Authentication
builder.Services.AddScoped<SimpleAuthProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SimpleAuthProvider>());

builder.Services.AddAuthorizationCore();

// JSON options
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

// App services
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IPostService, HttpPostService>();
builder.Services.AddScoped<ICommentService, HttpCommentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();