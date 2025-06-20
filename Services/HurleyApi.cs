namespace Hurley.Services;

public static class HurleyApi
{
    public static void Register(WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
            {
                message = "Welcome to hurleyAPI.",
                status = "OK",
                version = "v1",
                timestamp = DateTime.UtcNow
            }))
            .WithName("GetRoot")
            .WithDescription("Returns a basic greeting and confirms that hurleyAPI is operational.")
            .WithTags("General");
    }
}