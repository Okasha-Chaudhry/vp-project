var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/config", (IConfiguration configuration) => Results.Ok(new
{
    apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5000"
}));

app.MapFallbackToFile("index.html");

app.Run();
