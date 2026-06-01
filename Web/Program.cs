using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StudyConnect.Web;
using StudyConnect.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7000";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Authentication services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// App services
builder.Services.AddScoped<IAuthService, ClientAuthService>();
builder.Services.AddScoped<IGroupService, ClientGroupService>();
builder.Services.AddScoped<IResourceService, ClientResourceService>();
builder.Services.AddScoped<ISessionService, ClientSessionService>();
builder.Services.AddScoped<ITaskService, ClientTaskService>();

await builder.Build().RunAsync();
