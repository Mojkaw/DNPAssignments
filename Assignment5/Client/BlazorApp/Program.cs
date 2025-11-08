using BlazorApp.Components;
using BlazorApp.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Web API base address
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5101")
});

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

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

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();