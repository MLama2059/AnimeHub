using AnimeHubClient;
using AnimeHubClient.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Authentication Services
builder.Services.AddAuthorizationCore(); // Adds core authorization services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // Tells Blazor to use your custom provider
builder.Services.AddBlazoredLocalStorage(); // Registers the ILocalStorageService

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7114") });
builder.Services.AddScoped<IAuthService, AuthService>();

// Add MudBlazor services and configure snackbar defaults
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 2000; // ms
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

await builder.Build().RunAsync();
