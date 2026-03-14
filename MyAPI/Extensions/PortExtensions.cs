namespace MyAPI.Extensions;

public static class PortExtensions
{
    public static void ConfigurePort(this WebApplicationBuilder builder)
    {
        var port = Environment.GetEnvironmentVariable("PORT");

        if (!string.IsNullOrWhiteSpace(port))
        {
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
        }
    }
}