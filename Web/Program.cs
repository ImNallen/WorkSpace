using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using Web;
using Web.Clients;
using NetcodeHub.Packages.Extensions.SessionStorage;
using Web.Authentication;
using Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddNetcodeHubSessionStorageService();

#pragma warning disable S1075 // URIs should not be hardcoded
builder
    .Services.AddRefitClient<IAuthClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"));
#pragma warning restore S1075 // URIs should not be hardcoded

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
