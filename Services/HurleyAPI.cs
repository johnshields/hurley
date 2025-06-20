namespace HurleyAPI.Services;

public static class HurleyAPI
{
    public static void Register(WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
            {
                message = "Welcome to HurleyAPI.",
                status = "OK",
                version = "v1",
                timestamp = DateTime.UtcNow
            }))
            .WithName("GetRoot")
            .WithDescription("Returns a basic greeting and confirms that HurleyAPI is operational.")
            .WithTags("General");
    }
}