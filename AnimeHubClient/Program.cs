using AnimeHubClient;
using AnimeHubClient.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Read the URL from appsettings.json
string apiUrlStr = builder.Configuration["ApiBaseUrl"]
                   ?? "https://localhost:7114"; // Fallback if file missing
var apiUrl = new Uri(apiUrlStr);

// Authentication Services
builder.Services.AddAuthorizationCore(); // Adds core authorization services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // Tells Blazor to use your custom provider
builder.Services.AddBlazoredLocalStorage(); // Registers the ILocalStorageService
builder.Services.AddScoped<JwtAuthorizationMessageHandler>(); // Register the custom handler

builder.Services.AddHttpClient<IAnimeService, AnimeService>(client =>
    client.BaseAddress = apiUrl)
    // This line is critical for security: it attaches the JWT token
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
    client.BaseAddress = apiUrl)
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IRatingService, RatingService>(client =>
    client.BaseAddress = apiUrl)
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IWatchlistService, WatchlistService>(client =>
    client.BaseAddress = apiUrl)
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ILookUpService, LookUpService>(client =>
    client.BaseAddress = apiUrl);

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    // This ensures the HttpClient injected into AuthService knows the base URI (https://localhost:7114)
    client.BaseAddress = apiUrl);

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
